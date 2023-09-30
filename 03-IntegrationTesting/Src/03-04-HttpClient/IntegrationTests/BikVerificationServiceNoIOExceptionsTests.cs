using System;
using System.Linq;
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

/// <summary>
/// Klasa testowa wykorzystująca ręcznie ustawione wartości połączenia po HTTP.
/// W tym przypadku, domyślna implementacja BiKVerificationService, zareaguje na błąd
/// zalogowaniem informacji o wyjątku.
/// 
/// W tej klasie testowej pokazujemy jak powinniśmy przetestować naszego klienta HTTP.
/// Czy potrafimy obsłużyć wyjątki? Czy potrafimy obsłużyć scenariusze biznesowe?
///
/// O problemach związanych z pisaniem zaślepek przez konsumenta API, będziemy mówić
/// w dalszej części szkolenia. Tu pokażemy ręczne zaślepianie scenariuszy
/// biznesowych.
/// </summary>
public class BikVerificationServiceNoIoExceptionsTests
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

    //Przez konstruktor wstrzykujemy adres naszego serwera WireMock, zamiast
    //domślnego serwera Biura Informacji Kredytowej. Podajemy też skonfigurowaną
    //instancję klienta HTTP.
    _service = new BikVerificationService(
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
  public async Task ShouldReturnPositiveVerification()
  {
    // Zaślepiamy wywołanie GET, zwracając odpowiednią wartość tekstową
    _wireMockServer.Given(
        Request.Create()
          .WithPath("/18210116954")
          .UsingGet())
      .RespondWith(Response.Create()
        .WithBody("VerificationPassed"));

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationPassed);
  }

  [Test]
  public async Task ShouldReturnNegativeVerification()
  {
    // Zaślepiamy wywołanie GET, zwracając odpowiednią wartość tekstową
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithBody("VerificationFailed"));

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  // W tym i kolejnych testach zaślepiamy wywołanie GET zwracając różne
  // błędy techniczne. Chcemy się upewnić, że potrafimy je obsłużyć.
  // .NETowy Wiremock nie wspiera wszystkich błędów, które obsługuje
  // wersja Javowa, stąd przykładów jest o kilka mniej.
  [Test]
  public async Task ShouldFailWithMalformedResponseChunk()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithFault(FaultType.MALFORMED_RESPONSE_CHUNK)
    );

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  [Test]
  public async Task ShouldFailWithEmptyResponse()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithFault(FaultType.EMPTY_RESPONSE)
    );

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
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
}