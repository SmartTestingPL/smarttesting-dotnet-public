using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BikService.Infrastructure;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using NUnit.Framework;

namespace BikServiceTests;

// TODO: Dotyczy lekcji 05-04
[Category("chaos"), Explicit]
public class ChaosEngineeringTests
{
  private static int _port = 5141;

  [SetUp]
  public async Task SetUp()
  {
    await InvalidateCaches();
  }

  [TearDown]
  public async Task TearDown()
  {
    await DisableAttacks();
  }

  /// <summary>
  /// Hipoteza stanu ustalonego
  ///     POST na URL “/{pesel}”, z peselem osoby nie będącej oszustem, odpowie statusem 403, w ciągu 1000 ms
  /// Metoda
  ///     Włączamy błędy spowodowane integracją z bazą danych
  /// Wycofanie
  ///     Wyłączamy błędy spowodowane integracją z bazą danych
  /// </summary>
  [Test]
  public async Task ShouldReturn403Within1000MsWhenCheckingPeselWithDatabaseIssues()
  {

    await EnableExceptionAttack();

    var response = await Pesel(WithNonFraudPesel())
      .WithTimeout(1000.Milliseconds())
      .WithHeader("Content-Type", "application/json")
      .AllowAnyHttpStatus()
      .SendAsync(HttpMethod.Get);

    response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
    (await response.GetStringAsync()).Should()
      .Be(CustomerVerificationResult.Statuses.VerificationFailed.ToString());
  }

  private static Task EnableExceptionAttack()
  {
    return Chaos().AppendPathSegment("enableRepositoryExceptionAssault")
      .PostStringAsync(string.Empty);
  }

  private static IFlurlRequest Pesel(string pesel)
  {
    return new FlurlRequest($"http://localhost:{_port}").AppendPathSegment(pesel);
  }

  private static IFlurlRequest Chaos()
  {
    return new FlurlRequest($"http://localhost:{_port}")
      .AppendPathSegment("chaos");
  }

  private static string WithNonFraudPesel()
  {
    return "89050193724";
  }

  private static async Task DisableAttacks()
  {
    await Chaos().AppendPathSegment("clearAssaults")
      .PostStringAsync(string.Empty);
  }

  private static async Task InvalidateCaches()
  {
    await Chaos().AppendPathSegment("invalidateCaches")
      .PostStringAsync(string.Empty);
  }


}