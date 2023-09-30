using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TddXt.AnyRoot.Strings;
using WebApplication.Client;
using WebApplication.Lib;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

public class BikVerificationServiceTests
{
  /// <summary>
  /// Do tworzenia zaślepek używamy biblioteki WireMock
  /// </summary>
  private WireMockServer _wireMockServer = default!;

  /// <summary>
  /// Patrz metoda <see cref="SetUp"/>
  /// </summary>
  private BikVerificationService _service = default!;

  /// <summary>
  /// Klient HTTP
  /// </summary>
  private IFlurlClient _flurlClient = default!;

  /// <summary>
  /// W metodzie SetUp przed każdym testem uruchamiamy serwer WireMocka
  /// </summary>
  [SetUp]
  public void SetUp()
  {
    _flurlClient = new FlurlClient
    {
      Settings =
      {
        Timeout = 3.Seconds()
      }
    };
    _wireMockServer = WireMockServer.Start();

    // Przez konstruktor wstrzykujemy adres naszego serwera WireMock, zamiast
    // domyślnego serwera Biura Informacji Kredytowej. Podajemy też skonfigurowaną
    // instancję klienta HTTP. Nadpisujemy też metodę, obsługującą rzucony wyjątek.
    // W tym przypadku będziemy go ponownie rzucać.
    _service = new ThrowingBikVerificationService(
      _wireMockServer.Urls.Single(),
      _flurlClient,
      Any.Instance<ILogger<BikVerificationService>>());
  }

  /// <summary>
  /// Po każdym teście zatrzymujemy serwer WireMock.
  /// </summary>
  [TearDown]
  public void TearDown()
  {
    _wireMockServer.Stop();
  }

  [Test]
  public async Task ShouldFailWithTimeout()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithBody("VerificationPassed")
        // Zwracamy odpowiedź po 5 sekundach
        .WithDelay(5.Seconds()));

    // Oczekujemy, że zostanie rzucony wyjątek, związany z połączeniem HTTP
    // Gdyż według naszej konfiguracji połączenie HTTP powinno być nawiązane w
    // ciągu 3 sekund.
    (await _service.Awaiting(s => s.Verify(Zbigniew()))
        .Should().ThrowExactlyAsync<InvalidOperationException>())
      .WithInnerExceptionExactly<FlurlHttpTimeoutException>();
  }

  /// <summary>
  /// Wersja Javowa tego testu prezentowała sytuację, gdy
  /// serwer HTTP zwraca błąd MALFORMED_RESPONSE_CHUNK.
  /// W implementacji .NETowej, błędy takie jak EMPTY_RESPONSE
  /// czy MALFORMED_RESPONSE_CHUNK nie powodują wyjątków,
  /// więc wiremock w tym teście zwraca zwykły błąd.
  /// </summary>
  [Test]
  public async Task ShouldFailWithMalformedResponseChunk()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithStatusCode(HttpStatusCode.BadRequest)
    );

    (await _service.Awaiting(s => s.Verify(Zbigniew()))
        .Should().ThrowExactlyAsync<InvalidOperationException>())
      .WithInnerException<FlurlHttpException>();
  }

  private static Customer Zbigniew()
  {
    return new Customer(Guid.NewGuid(), YoungZbigniew());
  }

  private static Person YoungZbigniew()
  {
    return new Person(
      Any.String(),
      Any.String(),
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "18210116954",
      Guid.NewGuid());
  }

  /// <summary>
  /// Zaślepka testowa. W wersji Javowej jest klasą anonimową.
  /// </summary>
  private class ThrowingBikVerificationService : BikVerificationService
  {
    public ThrowingBikVerificationService(
      string bikServiceUri,
      IFlurlClient client,
      ILogger<BikVerificationService> logger)
      : base(bikServiceUri, client, logger)
    {
    }

    /// <summary>
    /// Symulujemy sytuację, w której nie obsłużyliśmy wyjątków
    /// </summary>
    /// <param name="exception">wyjątek do obsłużenia</param>
    protected override void ProcessException(Exception exception)
    {
      throw new InvalidOperationException("Invalid Operation", exception);
    }
  }
}