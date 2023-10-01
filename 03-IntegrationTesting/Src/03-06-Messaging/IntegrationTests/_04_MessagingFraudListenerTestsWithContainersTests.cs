using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Testcontainers.RabbitMq;
using WebApplication;
using WebApplication.Customers;
using WebApplication.Lib;
using WebApplication.Verifier.Customers;

namespace IntegrationTests;

/// <summary>
/// W tej klasie testowej piszemy test dla komponentu wysyłającego wiadomość
/// do brokera.
/// 
/// W teście `_03_CustomerVerifierWithContainersTests` już go przetestowaliśmy,
/// natomiast możemy przetestować go w izolacji. Wówczas w teście samego serwisu,
/// możemy użyć mocka. W ten sposób uzyskujemy ładny test integracyjny z wydzielonym
/// komponentem od wysyłki wiadomości.
/// 
/// Przed uruchomieniem właściwych testów, dzięki użyciu narzędzia
/// Testcontainers odpalimy w kontenerze Dockerowym, brokera RabbitMQ.
/// </summary>
public class _04_MessagingFraudListenerTestsWithContainersTests
{
  private RabbitMqContainer _rabbitMqTestContainer = default!;

  [OneTimeSetUp]
  public async Task InitializeContainer()
  {
    // Uruchomienie kontenera z brokerem na losowym porcie
    // przed uruchomieniem testów.
    _rabbitMqTestContainer =
      new RabbitMqBuilder()
        .WithCleanUp(true)
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _rabbitMqTestContainer.StartAsync();
  }

  [OneTimeTearDown]
  public Task DisposeOfContainer()
  {
    return _rabbitMqTestContainer.DisposeAsync().AsTask();
  }

  /// <summary>
  /// Test weryfikujący czy wysyłacz wiadomości potrafi wysłać wiadomość
  /// do brokera.
  /// </summary>
  [Test]
  public async Task ShouldStoreAFraudWhenACustomerVerificationWasReceivedFromAnExternalSystemAsync()
  {
    using var outputQueue = new FraudOutputQueue(_rabbitMqTestContainer.GetConnectionString());
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webHostConfig =>
        webHostConfig
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureAppConfiguration(builder =>
          {
            // dodajemy własną konfigurację, by nakazać aplikacji
            // komunikowanie się z naszym kontenerem dockerowym.
            // gdyby w appsettingsach było już to ustawienie,
            // ten kod by je nadpisał.
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
              {
                "RabbitMqConfiguration:ConnectionString",
                _rabbitMqTestContainer.GetConnectionString()
              }
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