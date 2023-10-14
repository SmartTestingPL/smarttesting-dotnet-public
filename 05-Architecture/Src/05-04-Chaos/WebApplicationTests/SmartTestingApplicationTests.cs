using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace WebApplicationTests;

/// <summary>
/// Klasa testowa wykorzystująca narzędzie Chaos Monkey do zmiany
/// zachowania naszej aplikacji w czasie rzeczywistym w celu weryfikacji
/// hipotez stanu ustalonego w ramach eksperymentów inżynierii chaosu.
/// 
/// Test jest opatrzony kategorią "chaos", po której można go odfiltrować.
/// Możemy mieć te testy w ramach naszej suity testów, ale niekoniecznie
/// zawsze będziemy chcieli automatycznie je uruchamiać.
/// </summary>
[Category("chaos")]
public class SmartTestingApplicationTests
{
  /// <summary>
  /// Testy uruchamiamy wobec działającej aplikacji na środowisku
  /// (pre)produkcyjnym. Musimy podać na jakim porcie znajduje się
  /// uruchomiona aplikacja.
  /// </summary>
  private const int Port = 5000;

  [TearDown]
  public Task Cleanup()
  {
    return DisableAttacks();
  }

  ///<summary>
  ///Hipoteza stanu ustalonego
  ///    POST na URL "fraud/fraudCheck", reprezentujący oszusta,
  ///    odpowie statusem 401, w ciągu 500 ms
  ///Metoda
  ///    Włączamy opóźnienie mające miejsce w kontrolerze
  ///Wycofanie
  ///    Wyłączamy opóźnienie mające miejsce w kontrolerze
  ///</summary>
  [Test]
  public async Task ShouldReturn401Within500MsWhenCallingFraudCheckWithIntroducedLatency()
  {
    await EnableLatencyAttack();

    var response = await FraudCheck().WithTimeout(500.Milliseconds())
      .WithHeader("Content-Type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(Fraud);

    response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
  }

  /// <summary>
  /// Hipoteza stanu ustalonego
  ///     POST na URL "fraud/fraudCheck", reprezentujący oszusta,
  ///     odpowie statusem 401, w ciągu 500 ms
  /// Metoda
  ///     Włączamy błędy spowodowane integracją z bazą danych
  /// Wycofanie
  ///     Wyłączamy błędy spowodowane integracją z bazą danych
  /// </summary>
  [Test]
  public async Task ShouldReturn401Within500MsWhenCallingFraudCheckWithDatabaseIssues()
  {
    await EnableExceptionAttack();

    var response = await FraudCheck().WithTimeout(500.Milliseconds())
      .WithHeader("Content-Type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(Fraud);

    response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
  }

  /// <summary>
  /// Po każdym teście wyłączamy ataki.
  /// </summary>
  private static Task DisableAttacks()
  {
    return Chaos().AppendPathSegment("clearAssaults")
      .PostStringAsync(string.Empty);
  }

  /// <summary>
  /// Włączenie wstrzykiwania opóźnień do kontrolerów
  /// za pomocą żądania HTTP
  /// </summary>
  private static Task EnableLatencyAttack()
  {
    return Chaos().AppendPathSegment("enableControllerLatencyAssault")
      .PostJsonAsync(new
      {
        Start = 1000,
        End = 3000
      });
  }

  /// <summary>
  /// Włączenie wstrzykiwania wyjątków do repozytorium
  /// za pomocą żądania HTTP
  /// </summary>
  private static Task EnableExceptionAttack()
  {
    return Chaos().AppendPathSegment("enableRepositoryExceptionAssault")
      .PostStringAsync(string.Empty);
  }

  private static string FraudCheck()
  {
    return ("http://localhost:" + Port)
      .AppendPathSegment("fraud")
      .AppendPathSegment("fraudCheck");
  }

  private static string Chaos()
  {
    return ("http://localhost:" + Port)
      .AppendPathSegment("chaos");
  }

  private static readonly string Fraud = new JObject(
    new JProperty("Guid", "48d80d4a-5ea9-4685-b241-e75d5dca9a63"),
    new JProperty("Person",
      new JObject(
        new JProperty("Name", "Fradeusz"),
        new JProperty("Surname", "Fraudowski"),
        new JProperty("DateOfBirth", "1980-01-01"),
        new JProperty("Gender", "Male"),
        new JProperty("NationalIdentificationNumber", "2345678901"))
    )
  ).ToString();
}