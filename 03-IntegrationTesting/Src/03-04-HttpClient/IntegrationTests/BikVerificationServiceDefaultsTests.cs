using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
/// Klasa testowa wykorzystująca wartości "zepsute" (w Javowym kliencie Apache - domyślne)
/// w konfiguracji biblioteki do tworzenia połączeń po HTTP.
/// </summary>
public class BikVerificationServiceDefaultsTests
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
  /// W metodzie SetUp przed każdym testem uruchamiamy serwer WireMocka
  /// </summary>
  [SetUp]
  public void SetUp()
  {
    _wireMockServer = WireMockServer.Start();
    _service = new BikVerificationService(
      _wireMockServer.Urls.Single(),
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

  /// <summary>
  /// Jeśli odkomentujemy ten test - nigdy się nie zakończy. W wersji Javowej wynikało
  /// to z pewnych ustawień domyślnych na kliencie HTTP. Tu musiałem zasymulować taką sytuację,
  /// ustawiając nieskończony czas wygaśnięcia żądania na domyślnym FlurlCliencie
  /// tworzonym w klasie BikVerificationService.
  /// </summary>
  //[Test]
  public async Task ShouldFailWithExceptionThrown()
  {
    // Mówimy zaślepce serwera HTTP, żeby nie odpowiadała przez określony czas.
    // Dobrze skonfigurowany klient HTTP powinien rzucić wyjątkiem po określonym czasie.

    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithCallback(async message =>
        {
          // (w wersji Javowej był tutaj błąd Connection Reset By Peer, ale .NETowy Wiremock
          // nie wspiera tego błędu).
          await Task.Delay(Timeout.InfiniteTimeSpan);
          return null!;
        })
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