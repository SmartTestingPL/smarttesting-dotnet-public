using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Model;
using FluentAssertions;
using Flurl.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebApplication;

namespace WebApplicationTests;

/// <summary>
/// Klasa testująca API HTTP. Wykorzystuje TestServer w celu
/// przetestowania bez faktycznego połączenia po warstwie sieciowej.
/// </summary>
public class _01_FraudControllerTests
{
  private IHost _host = default!;

  /// <summary>
  /// Przed każdym uruchomieniem testu ustawiamy TestServer na nowo.
  /// </summary>
  /// <returns></returns>
  [SetUp]
  public Task SetUp()
  {
    _host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureTestServices(collection =>
            // na potrzeby tych testów zastępujemy prawdziwą logikę
            // naszym fałszywym obiektem.
            collection.Replace(
              new ServiceDescriptor(
                typeof(ICustomerVerifier),
                context => new FakeTestVerifier(),
                ServiceLifetime.Transient))))
      .Build();
    return _host.StartAsync();
  }

  [TearDown]
  public void TearDown()
  {
    _host.Dispose();
  }

  /// <summary>
  /// Prosty test weryfikujący nasze API dla przypadku oszusta.
  /// </summary>
  [Test]
  public async Task ShouldReturnFraud()
  {
    using var client = new FlurlClient(_host.GetTestClient());

    using var response = await client.Request("fraud", "fraudCheck")
      .AllowAnyHttpStatus()
      .WithHeader("Content-Type", "application/json")
      .PostStringAsync(
        new JObject(
          new JProperty("Guid", "48d80d4a-5ea9-4685-b241-e75d5dca9a63"),
          new JProperty("Person",
            new JObject(
              new JProperty("Name", "Fradeusz"),
              new JProperty("Surname", "Fraudowski"),
              new JProperty("DateOfBirth", "1980-01-01"),
              new JProperty("Gender", "Male"),
              new JProperty("NationalIdentificationNumber", "2345678901")
            )
          )).ToString());

    response.StatusCode.Should().Be((int) HttpStatusCode.Unauthorized);
  }

  /// <summary>
  /// Prosty test weryfikujący nasze API dla przypadku oszusta.
  /// Dodatkowo, tworzy dokumentację naszego API.
  /// 
  /// W .Net Core nie ma mechanizmu a'la RestDoc, który jest zademonstrowany
  /// w wersji Javowej tego przykładu, ale można go spróbować
  /// "chałupniczo" zaimplementować. Tu pokazana jest taka prymitywna
  /// implementacja w postaci klasy <see cref="Doc"/>, która podłącza
  /// się pod zdarzenia klienta HTTP.
  /// </summary>
  [Test]
  public async Task ShouldReturnFraudWithHttpDocs()
  {
    //Tworzymy klase dokumentu, podając jej nazwę pliku i opis scenariusza.
    await using var doc = new Doc("fraudCheck", "Basic fraud detection scenario.");
    using var client = new FlurlClient(_host.GetTestClient());

    // Podpinamy dokument pod zdarzenia klienta HTTP
    Doc.Connect(client, doc);

    using var response = await client.Request("fraud", "fraudCheck")
      .AllowAnyHttpStatus()
      .WithHeader("Content-Type", "application/json")
      .PostStringAsync(
        new JObject(
          new JProperty("Guid", "48d80d4a-5ea9-4685-b241-e75d5dca9a63"),
          new JProperty("Person",
            new JObject(
              new JProperty("Name", "Fradeusz"),
              new JProperty("Surname", "Fraudowski"),
              new JProperty("DateOfBirth", "1980-01-01"),
              new JProperty("Gender", "Male"),
              new JProperty("NationalIdentificationNumber", "2345678901")
            )
          )).ToString());

    response.StatusCode.Should().Be((int) HttpStatusCode.Unauthorized);
  }

  [Test]
  public void ShouldReturnFraudWithRestDocsAndSpringCloudContract()
  {
    // Pod .NET Core nie ma narzędzia które byłoby w stanie na podstawie
    // testów wygenerować jednocześnie fragmenty komunikacji, zaślepki
    // oraz kontrakty, stąd test nr 3 z oryginału Javowego w tej wersji
    // się nie pojawia.
  }

}

/// <summary>
/// Na potrzeby testów uznajemy, że pan Fraudowski reprezentuje oszusta.
/// Każdy inny przypadek to osoba uczciwa.
/// </summary>
public class FakeTestVerifier : ICustomerVerifier
{
  public Task<IReadOnlyList<VerificationResult>> Verify(Customer customer, CancellationToken cancellationToken)
  {
    IReadOnlyList<VerificationResult> result 
      = customer.Person.Surname == "Fraudowski" 
        ? new List<VerificationResult> {new("test", false)} 
        : new List<VerificationResult> {new("test", true)};
    return Task.FromResult(result);
  }
}