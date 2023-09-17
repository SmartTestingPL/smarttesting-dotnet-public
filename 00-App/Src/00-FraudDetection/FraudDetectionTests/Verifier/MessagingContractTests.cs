using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtmaFileSystem;
using Core.Maybe;
using FraudDetection;
using FraudDetection.Customers;
using FraudDetection.Verifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NSubstitute;
using NUnit.Framework;
using PactNet.Verifier;
using Testcontainers.RabbitMq;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 05-03, odpowiednik Javowego MessagingBase
public class MessagingContractTests
{
  private RabbitMqContainer _rabbitMqTestContainer = default!;
  private FraudOutputQueue _outputQueue = default!;
  private IHost _host = default!;
  private IFraudAlertNotifier _fraudAlertNotifier = default!;

  [SetUp]
  public async Task SetUp()
  {
    _rabbitMqTestContainer =
      new RabbitMqBuilder()
        .WithCleanUp(true)
        .WithImage("rabbitmq:3.7.25-management-alpine")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _rabbitMqTestContainer.StartAsync();

    _outputQueue = new FraudOutputQueue(_rabbitMqTestContainer.GetConnectionString());
    _host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webHostConfig =>
        webHostConfig
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureServices(ctx => { ctx.AddSingleton(Substitute.For<IVerificationRepository>()); })
          .ConfigureAppConfiguration(builder =>
          {
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
              ["RabbitMqOptions:ConnectionString"] = _rabbitMqTestContainer.GetConnectionString()
            });
          }))
      .Build();
    await _host.StartAsync();

    _fraudAlertNotifier = _host.Services.GetRequiredService<IFraudAlertNotifier>();
  }

  [TearDown]
  public async Task TearDown()
  {
    _outputQueue.Dispose();
    await _host.StopAsync();
    _host.Dispose();
    await _rabbitMqTestContainer.DisposeAsync();
  }

  [Test]
  public void ShouldHonourPactWithConsumer()
  {
    _fraudAlertNotifier.FraudFound(new CustomerVerification(
      new Person(
        "Fraudeusz", 
        "Fraudowski", 
        new LocalDate(1980, 01, 01).Just(), 
        Gender.Male, 
        "2345678901"),
      new CustomerVerificationResult(
        Guid.Parse("cc8aa8ff-40ff-426f-bc71-5bb7ea644108"),
        VerificationStatus.VerificationFailed)));

    using var verifier = new PactVerifier();
    verifier
      .MessagingProvider("FraudDetection")
      .WithProviderMessages(scenarios =>
      {
        scenarios.Add("should produce a fraud found event", () => new List<CustomerVerification>
        {
          _outputQueue.Receive().Value()
        });
      })
      .WithFileSource(AbsoluteFilePath.OfThisFile()
        .ParentDirectory(3).Value()
        .AddDirectoryName("Contracts")
        .AddDirectoryName("Messaging")
        .AddFileName("CustomerVerificationEventConsumer-FraudDetection.json").Info())
      .Verify();
  }
}