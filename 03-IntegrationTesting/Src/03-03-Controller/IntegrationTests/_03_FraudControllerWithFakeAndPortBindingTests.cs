using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebApplication;
using WebApplication.Client;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa do slajdu z testowaniem kontrolera po warstwie HTTP
/// z alokacją portu z zamockowanym serwisem aplikacyjnym.
/// Wykorzystując Asp.Net Core, za pomocą naszego Hosta
/// startujemy serwer aplikacyjny z serwerem HTTP uruchomionym na losowym porcie.
///
/// W tym teście nie traktujemy kontrolera jako obiektu. Wyślemy prawdziwe żądanie HTTP
/// i z weryfikujemy czy otrzymujemy rezultat, który nas interesuje.
///
/// Nadpisujemy rejestrację serwisu aplikacyjnego, która zwraca wartości "na sztywno".
/// Gdybyśmy w którymś z komponentów mieli połączenie z bazą danych,
/// NIE zostałoby ono zrealizowane.
/// </summary>
public class _03_FraudControllerWithFakeAndPortBindingTests
{
  /// <summary>
  /// Stawiamy serwer, po czym pobieramy jego adres (port był przecież generowany).
  /// Wykorzystując klienta HTTP (tutaj Flurl), wysyłamy żądanie HTTP
  /// ze zbyt młodym Zbigniewem i oczekujemy, że dostaniemy status 401.
  /// </summary>
  [Test]
  public async Task ShouldRejectLoanApplicationWhenPersonTooYoung()
  {
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          //metoda pomocnicza do testów integracyjnych
          //- pozwala podmienić zależności w kontenerze.
          .ConfigureTestServices(collection =>
          {
            //zamieniamy prawdziwą implementację interfejsu ICustomerVerifier na naszą
            collection.Replace(
              new ServiceDescriptor(
                typeof(ICustomerVerifier),
                typeof(FakeCustomerVerifier),
                ServiceLifetime.Singleton));
          })
          .UseStartup<Startup>()
          .UseKestrel()
          .UseUrls("http://[::1]:0"))
      .Build();
    await host.StartAsync();

    var address = RetrieveAppBaseUrl(host);

    using var httpResponseMessage = await address.AppendPathSegments("fraud", "fraudCheck")
      .WithHeader("content-type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(TooYoungZbigniew());

    httpResponseMessage.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
  }

  /// <summary>
  /// Metoda umożliwiająca pobranie adresu, na którym wystartowała nasza aplikacja
  /// </summary>
  private static string RetrieveAppBaseUrl(IHost host)
  {
    var server = host.Services.GetRequiredService<IServer>();
    var addressFeature = server.Features.GetRequiredFeature<IServerAddressesFeature>();
    return addressFeature.Addresses.Single();
  }

  private static string TooYoungZbigniew()
  {
    return new JObject(
      new JProperty("guid", "7b3e02b3-6b1a-4e75-bdad-cef5b279b074"),
      new JProperty("name", "Zbigniew"), 
      new JProperty("surname", "Zamłodowski"),
      new JProperty("dateOfBirth", "2019-01-01"), 
      new JProperty("gender", "Male"),
      new JProperty("nationalIdentificationNumber", "18210116954")).ToString();
  }
}