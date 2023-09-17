using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Maybe;
using FluentAssertions;
using FraudDetection;
using FraudDetection.Customers;
using FraudDetection.Lib;
using FraudDetection.Verifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using Testcontainers.RabbitMq;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 03-06
public class MessagingFraudAlertNotifierTests
{
  private RabbitMqContainer _rabbitMqTestContainer = default!;

  [OneTimeSetUp]
  public async Task InitializeContainer()
  {
    _rabbitMqTestContainer =
      new RabbitMqBuilder()
        .WithCleanUp(true)
        .WithImage("rabbitmq:3.7.25-management-alpine")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _rabbitMqTestContainer.StartAsync();
  }

  [OneTimeTearDown]
  public async Task DisposeOfContainer()
  {
    await _rabbitMqTestContainer.DisposeAsync();
  }

  [Test]
  public async Task ShouldStoreAFraudWhenACustomerVerificationWasReceivedFromAnExternalSystemAsync()
  {
    using var outputQueue = new FraudOutputQueue(_rabbitMqTestContainer.GetConnectionString());
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webHostConfig =>
        webHostConfig
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureServices(ctx =>
          {
            ctx.AddSingleton(Substitute.For<IVerificationRepository>());
          })
          .ConfigureAppConfiguration(builder =>
          {
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
              ["RabbitMqOptions:ConnectionString"] = _rabbitMqTestContainer.GetConnectionString()
            });
          }))
      .Build();
    await host.StartAsync();

    var fraudAlertNotifier = host.Services.GetRequiredService<IFraudAlertNotifier>();

    var zbigniew = Zbigniew();
    fraudAlertNotifier.FraudFound(CustomerVerification(zbigniew));

    var receivedMessage = outputQueue.Receive();
    receivedMessage.HasValue.Should().BeTrue();
    receivedMessage.Value().Result.UserId.Should().Be(zbigniew.Guid);
  }

  private static CustomerVerification CustomerVerification(Customer zbigniew)
  {
    return new CustomerVerification(zbigniew.Person, CustomerVerificationResult.Failed(zbigniew.Guid));
  }

  private static Customer Zbigniew()
  {
    return new Customer(
      Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"), 
      YoungZbigniew());
  }

  private static Person YoungZbigniew()
  {
    return new Person(
      "Zbigniew",
      "Zbigniewowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "18210116954");
  }
}