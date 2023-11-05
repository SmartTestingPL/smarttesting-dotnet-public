using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using FluentAssertions;
using FluentAssertions.Extensions;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Polly;
using WebApplication;

namespace WebApplicationTests;

/// <summary>
/// Klasa testowa spinająca konfiguracje aplikacji w celu
/// weryfikacji przetwarzania na wielu wątkach.
/// </summary>
public class _02_CustomerVerifierTests
{
  private _03_VerificationListener _verificationListener = default!;
  private ServiceProvider _container = default!;
  private _01_CustomerVerifier _verifier = default!;

  [SetUp]
  public void SetUp()
  {
    var containerBuilder = new ServiceCollection();
    Startup.AddDependenciesTo(containerBuilder);
    _container = containerBuilder.BuildServiceProvider();
    _verificationListener = _container
      .GetRequiredService<_03_VerificationListener>();
    _verifier = _container.GetRequiredService<_01_CustomerVerifier>();
  }

  [TearDown]
  public void TearDown()
  {
    _container.Dispose();
  }

  /// <summary>
  /// Test uruchamia procesowanie klienta Stefan. Przetwarzanie zostanie
  /// ukończone w losowej kolejności, natomiast w naszym teście oczekujemy,
  /// że dostaniemy odpowiedź w kolejności weryfikacji
  /// age, id i na końcu name.
  /// 
  /// Na wszelki wypadek uruchamiamy test 5 razy, żeby upewnić się, że
  /// za każdym razem przejdzie.
  /// 
  /// Zakomentuj atrybut <see cref="IgnoreAttribute"/>, żeby przekonać się,
  /// że test może nie przejść!
  /// </summary>
  [Test]
  [Ignore("fails")]
  [Repeat(5)]
  public async Task ShouldReturnResultsInOrderOfExecution()
  {
    var results = await _verifier.Verify(
      new Customer(
        Guid.NewGuid(),
        TooYoungStefan()),
      new CancellationToken());

    results.Select(r => r.VerificationName)
      .Should().Equal("age", "id", "name");
  }

  /// <summary>
  /// Test uruchamia procesowanie klienta Stefan. Procesowanie zostanie
  /// ukończone w losowej kolejności. W naszym teście oczekujemy, że
  /// dostaniemy odpowiedź zawierającą wszystkie 3 weryfikacje w losowej
  /// kolejności.
  /// </summary>
  [Test]
  [Repeat(5)]
  public async Task ShouldWorkInParallelWithLessConstraint()
  {
    var results = await _verifier.Verify(
      new Customer(
        Guid.NewGuid(),
        TooYoungStefan()),
      new CancellationToken());

    results.Select(r => r.VerificationName)
      .Should().BeEquivalentTo("age", "id", "name");
  }

  /// <summary>
  /// Testujemy asynchroniczne procesowanie zdarzeń. Po każdej weryfikacji
  /// zostaje wysłane zdarzenie, które komponent
  /// <see cref="_03_VerificationListener"/> trzyma w kolejce.
  /// 
  /// Przetwarzanie jest asynchroniczne, a test nie bierze tego pod uwagę.
  /// Po zakolejkowaniu wywołania wywołań asynchronicznych od razu przechodzi
  /// do asercji zapisanych zdarzeń w kolejce. Problem w tym, że przetwarzanie
  /// jeszcze trwa! Innymi słowy test jest szybszy niż kod, który testuje.
  /// 
  /// Zakomentuj atrybut <see cref="IgnoreAttribute"/>, żeby przekonać się,
  /// że test może nie przejść!
  /// </summary>
  [Test]
  [Ignore("fails")]
  [Repeat(5)]
  public void ShouldWorkInParallelWithoutASleep()
  {
    _verifier.VerifyAsync(new Customer(Guid.NewGuid(), TooYoungStefan()));

    _verificationListener.Events.Select(e => e.SourceDescription)
      .Should().BeEquivalentTo("age", "id", "name");
  }

  /// <summary>
  /// Próba naprawy sytuacji z testu powyżej.
  /// 
  /// Zakładamy, że w ciągu 4 sekund zadania powinny się ukończyć,
  /// a zdarzenia powinny zostać wysłane.
  /// 
  /// Rozwiązanie to w żaden sposób się nie skaluje i jest marnotrawstwem
  /// czasu. W momencie, w którym przetwarzanie ukończy się po np. 100 ms,
  /// zmarnujemy 3.9 sekundy czekając, by móc dokonać asercji.
  /// </summary>
  [Test]
  [Repeat(5)]
  public async Task ShouldWorkInParallelWithASleep()
  {
    _verifier.VerifyAsync(new Customer(Guid.NewGuid(), TooYoungStefan()));

    await Task.Delay(4.Seconds());

    _verificationListener.Events.Select(e => e.SourceDescription)
      .Should().BeEquivalentTo("age", "id", "name");
  }

  /// <summary>
  /// Najlepsze rozwiązanie problemu.
  /// 
  /// Wykorzystujemy bibliotekę Polly, która stosuje polling - czyli poczeka
  /// maksymalnie 5 sekund i będzie co 100 milisekund weryfikować rezultat
  /// asercji. W ten sposób maksymalnie będziemy spóźnieni wobec uzyskanych
  /// wyników o ok. 100 ms.
  /// </summary>
  [Test]
  [Repeat(5)]
  public void ShouldWorkInParallelWithPolly()
  {
    _verifier.VerifyAsync(new Customer(Guid.NewGuid(), TooYoungStefan()));

    Policy.Handle<AssertionException>()
      .WaitAndRetry(50, retryAttempt => 100.Milliseconds())
      .Execute(() =>
      {
        return _verificationListener.Events.Select(e => e.SourceDescription)
          .Should().BeEquivalentTo(new[] { "age", "id", "name" });
      });
  }

  private static Person TooYoungStefan()
  {
    return new Person(
      "",
      "",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "0123456789");
  }
}