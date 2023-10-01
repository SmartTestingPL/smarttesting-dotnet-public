using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testcontainers.RabbitMq;
using WebApplication;
using WebApplication.Customers;
using WebApplication.Lib;
using WebApplication.Verifier.Customers;
using RabbitMqConfiguration = WebApplication.RabbitMqConfiguration;

namespace IntegrationTests;

/// <summary>
/// W tej klasie testowej piszemy test dla komponentu
/// nasłuchującego wiadomości z brokera.
/// Przed uruchomieniem właściwych testów, dzięki użyciu narzędzia
/// Testcontainers odpalimy w kontenerze Dockerowym brokera RabbitMQ.
/// </summary>
public class _03_MessagingFraudListenerTests
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
  public async Task DisposeOfContainer()
  {
    await _rabbitMqTestContainer.DisposeAsync();
  }

  /// <summary>
  /// Test weryfikujący czy nasłuchiwacz wiadomości, potrafi pobrać wiadomość
  /// z brokera i zapisać do bazy danych. Na potrzeby szkolenia i prostoty
  /// przykładu nie stawiamy bazy danych w kontenerze, żeby zweryfikować czy
  /// w bazie faktycznie odłożył się klient. Można jednak byłoby rozważyć taki test,
  /// gdyż klasa nasłuchująca jest bardzo prosta i rozbicie jej na dwa osobne testy
  /// wydaje się nadgorliwością.
  /// </summary>
  [Test]
  public async Task ShouldStoreAFraudWhenACustomerVerificationWasReceivedFromAnExternalSystemAsync()
  {
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(webHostconfig =>
        webHostconfig
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

    var customer = Stefania();

    // Wysyłamy wiadomość na kolejkę
    CreateRabbitMqDestination().Send("fraudInput", FailedStefaniaVerification(customer));

    // Komunikacja jest asynchroniczna, oczekujemy, że w końcu wiadomość zostanie
    // skonsumowana i klient zostanie zapisany w bazie danych. 
    // NUnit udostępnia asercję, która pozwala to zrobić. Można też użyć biblioteki Polly
    // (będą jeszcze przykłady na użycie tej biblioteki).
    Assert.That(() =>
        // Jeden DbContext powinien być używany tylko przez 1 wątek/task.
        // Tutaj pobieramy jeden dla testu, więc w kontenerze IoC
        // powinien być zarejestrowany jako przejściowy (transient)
        host.Services.GetRequiredService<IVerificationRepository>()
          .FindByUserId(customer.Guid).HasValue,
      Is.True.After(10000, 500));
  }

  private RabbitMqDestination CreateRabbitMqDestination()
  {
    return new RabbitMqDestination(Options.Create(new RabbitMqConfiguration
    {
      ConnectionString = _rabbitMqTestContainer.GetConnectionString()
    }));
  }

  private static CustomerVerification FailedStefaniaVerification(Customer customer)
  {
    return new CustomerVerification(
      customer.Person, 
      CustomerVerificationResult.Failed(customer.Guid));
  }

  private static Customer Stefania()
  {
    return new Customer(
      Guid.Parse("789b58b8-957b-4f76-8046-1287382b2e64"), 
      YoungStefania());
  }

  private static Person YoungStefania()
  {
    return new Person(
      "Stefania",
      "Stefanowska",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Female,
      "18210145358");
  }
}