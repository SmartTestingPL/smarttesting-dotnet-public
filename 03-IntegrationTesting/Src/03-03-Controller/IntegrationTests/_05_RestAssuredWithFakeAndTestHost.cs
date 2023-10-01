using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RA;
using RA.Enums;
using WebApplication;
using WebApplication.Client;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa do slajdu z testowaniem frameworka opartego o BDD: Rest Assured
/// i jego integracji z HttpClientem. W tym przykładzie testujemy tylko kontroler.
///
/// Wersja RestAssured pod .Net nie jest jakoś mocno wspierana
/// i zanim jej użyjesz, zastanów się, czy nie wystarczy Ci
/// Flurl.Http + FluentAssertions. Wersja .Net nie ma również takiej
/// integracji z frameworkiem, jaką pokazują przykłady Javowe.
/// 
/// Natomiast można użyć RestAssured z TestHostem i poniższy przykład
/// pokazuje jak można to osiągnąć.
/// </summary>
public class _05_RestAssuredWithFakeAndTestHost
{
  /// <summary>
  /// Opisujemy za pomocą nomenklatury pseudo-Gherkinowej
  /// (https://github.com/cucumber/cucumber/tree/master/gherkin), jak chcielibyśmy,
  /// żeby API działało.
  ///
  /// W sekcji HttpClient() przekazujemy klienta HTTP. Może to być zarówno prawdziwy klient,
  /// jak i (tak jak w tym przypadku) klient pozyskany z TestServera.
  /// </summary>
  /// <returns></returns>
  [Test]
  public async Task ShouldRejectLoanApplicationWhenPersonTooYoung()
  {
    using var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureTestServices(collection =>
          {
            collection.Replace(
              new ServiceDescriptor(
                typeof(ICustomerVerifier),
                typeof(FakeCustomerVerifier),
                ServiceLifetime.Singleton));
          })
      )
      .Build();
    await host.StartAsync();


    var testClient = host.GetTestClient();
    new RestAssured()
      .Given()
      .HttpClient(testClient)
      .Header(HeaderType.ContentType.Value, "application/json")
      .Body(TooYoungZbigniew())
      .When()
      .Post($"{testClient.BaseAddress}fraud/fraudCheck")
      .Then()
      .TestStatus(
        "Status code should be Unauthorized",
        code => code == (int)HttpStatusCode.Unauthorized)
      .AssertAll();
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