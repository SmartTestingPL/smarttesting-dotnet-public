using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BikService.Infrastructure;
using BikServiceTests.Acceptance;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using NUnit.Framework;

namespace BikServiceTests;

// TODO: Dotyczy lekcji 05-04
[Category("chaos"), Explicit]
public class ChaosEngineeringLocalTests : Acceptance.Infrastructure
{
  private BikWebAppForAcceptanceTests _app = default!;

  [SetUp]
  public async Task SetUp()
  {
    await Rabbit.StartAsync();
    await MongoDbContainer.StartAsync();
    await PostgreSql.StartAsync();

    StartHttpServers();
    StubServices();
    _app = StartApp();
    await InvalidateCaches();
  }

  [TearDown]
  public async Task TearDown()
  {
    await DisableAttacks();
    await _app.DisposeAsync();
    ShutdownServices();
    await Rabbit.DisposeAsync();
    await MongoDbContainer.DisposeAsync();
    await PostgreSql.DisposeAsync();
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

  private Task EnableExceptionAttack()
  {
    return Chaos().AppendPathSegment("enableRepositoryExceptionAssault")
      .PostStringAsync(string.Empty);
  }

  private IFlurlRequest Pesel(string pesel)
  {
    return _app.Request(pesel);
  }

  private IFlurlRequest Chaos()
  {
    return _app.Request("chaos");
  }

  private static string WithNonFraudPesel()
  {
    return "89050193724";
  }

  private async Task DisableAttacks()
  {
    await Chaos().AppendPathSegment("clearAssaults")
      .PostStringAsync(string.Empty);
  }

  private async Task InvalidateCaches()
  {
    await Chaos().AppendPathSegment("invalidateCaches")
      .PostStringAsync(string.Empty);
  }
}