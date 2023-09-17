using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using NUnit.Framework;

namespace BikServiceTests.Acceptance;

// TODO: Dotyczy lekcji 04-01
public class AcceptanceTests : Infrastructure
{
  [SetUp]
  public async Task SetUp()
  {
    await Rabbit.StartAsync();
    await MongoDbContainer.StartAsync();
    await PostgreSql.StartAsync();

    StartHttpServers();
    StubServices();
  }

  [TearDown]
  public async Task TearDown()
  {
    ShutdownServices();

    await Rabbit.DisposeAsync();
    await MongoDbContainer.DisposeAsync();
    await PostgreSql.DisposeAsync();
  }

  [Test]
  public async Task ShouldReturnFailedVerificationWhenFraudAppliesForACheck()
  {
    await using var app = StartApp();

    var response = await app.Request(Pesel(WithFraudPesel()))
      .AllowAnyHttpStatus()
      .GetAsync();

    response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
    (await response.GetStringAsync()).Should().Contain("status\":\"VerificationFailed\"");
  }

  [Test]
  public async Task ShouldReturnPassedVerificationWhenNonFraudAppliesForACheck()
  {
    await using var app = StartApp();

    var response = await app.Request(Pesel(WithNonFraudPesel()))
      .GetAsync();

    response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    (await response.GetStringAsync()).Should().Contain("status\":\"VerificationPassed\"");
  }

  private static string WithNonFraudPesel() => "89050193724";
  private static string WithFraudPesel() => "12345678901";
  private static string Pesel(string pesel) => $"/{pesel}";
}