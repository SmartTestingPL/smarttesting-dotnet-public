using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Maybe;
using FluentAssertions;
using FluentAssertions.Extensions;
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
using TddXt.XNSubstitute;
using Testcontainers.RabbitMq;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 03-06
public class MessagingFraudListenerTests
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
  public async Task ShouldStoreAFraudWhenACustomerVerificationWasReceivedFromAnExternalSystem()
  {
    var verificationRepository = Substitute.For<IVerificationRepository>();
    using var outputQueue = new FraudOutputQueue(_rabbitMqTestContainer.GetConnectionString());
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webHostConfig =>
        webHostConfig
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureAppConfiguration(builder =>
          {
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
              ["RabbitMqOptions:ConnectionString"] = _rabbitMqTestContainer.GetConnectionString()
            });
          }))
      .ConfigureServices(ctx =>
      {
        ctx.AddSingleton(verificationRepository);
      })
      .Build();
    await host.StartAsync();

    var fraudAlertNotifier = host.Services.GetRequiredService<IFraudDestination>();

    var customer = Stefania();
    fraudAlertNotifier.Send("fraudInput", FailedStefaniaVerification(customer));

    await this.Awaiting(_ => verificationRepository.Received(1).SaveAsync(
      Arg<VerifiedPerson>.That(person => person.UserId.Should().Be(customer.Guid))))
      .Should().NotThrowAfterAsync(10.Seconds(), 1.Seconds());
  }

  private CustomerVerification FailedStefaniaVerification(Customer customer) 
  {
    return new CustomerVerification(
      customer.Person, 
      CustomerVerificationResult.Failed(customer.Guid));
  }

  private Customer Stefania() 
  {
    return new Customer(
      Guid.Parse("789b58b8-957b-4f76-8046-1287382b2e64"), 
      YoungStefania());
  }

  private Person YoungStefania() 
  {
    return new Person(
      "Stefania", 
      "Stefanowska", 
      new LocalDate().Just(), 
      Gender.Female, 
      "18210145358");
  }

}