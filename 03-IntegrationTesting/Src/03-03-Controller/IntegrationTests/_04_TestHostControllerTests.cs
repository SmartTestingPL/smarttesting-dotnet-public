using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebApplication;
using WebApplication.Client;

namespace IntegrationTests;

///<summary>
/// Klasa testowa do slajdu z testowaniem kontrolera z zamockowaną warstwą HTTP.
/// Wykorzystując Asp.Net Core, za pomocą mechanizmów z nugeta TestHost,
/// uruchamiamy nasz Startup. Ponadto, oczekujemy, że warstaw sieciowa zostanie zamockowana
/// (wywołanie metody pomocniczej UseTestServer()).
/// 
/// W tym teście nie traktujemy kontrolera jako obiektu. Wyślemy zamockowane żądanie HTTP
/// i zweryfikujemy czy otrzymujemy rezultat, który nas interesuje.
/// 
/// Nadpisujemy rejestrację serwisu aplikacyjnego, która zwraca wartości "na sztywno".
/// Gdybyśmy w którymś z komponentów mieli połączenie z bazą danych,
/// NIE zostałoby ono zrealizowane.
/// </summary>
public class _04_TestHostControllerTests
{
  /// <summary>
  /// Stawiamy fałszywy serwer. Wykorzystując klienta HTTP (tutaj Flurl, uwaga, opakowujący
  /// HttpClienta którego dostarcza nam TestServer!), wysyłamy fałszywe żądanie HTTP
  /// ze zbyt młodym Zbigniewem i oczekujemy, że dostaniemy status 401.
  ///
  /// Komunikacja nie przebiega po prawdziwym HTTP.
  /// </summary>
  [Test]
  public async Task ShouldRejectLoanApplicationWhenPersonTooYoung()
  {
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          //używamy TestServera - rozszerzenia do testów integracyjnych
          .UseTestServer()
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
          }))
      .Build();
    await host.StartAsync();

    //opakowujemy klientem Flurla HttpClienta którego daje nam TestServer!
    using var client = new FlurlClient(host.GetTestServer().CreateClient());

    using var httpResponseMessage = await client.Request("fraud", "fraudCheck")
      .WithHeader("content-type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(TooYoungZbigniew());

    httpResponseMessage.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
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