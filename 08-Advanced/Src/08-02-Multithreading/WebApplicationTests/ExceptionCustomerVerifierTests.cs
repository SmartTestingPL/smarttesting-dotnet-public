using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using FluentAssertions;
using FluentAssertions.Extensions;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Polly;
using WebApplication;

namespace WebApplicationTests;

/// <summary>
/// Testowa implementacja weryfikacji, która będzie rzucać wyjątkiem.
/// </summary>
public class ExceptionCustomerVerifierTests
{
  private ExceptionThrowingFraudNotifier _verificationNotifier = default!;
  private ServiceProvider _container = default!;
  private _01_CustomerVerifier _verifier = default!;

  [SetUp]
  public void SetUp()
  {
    var containerBuilder = new ServiceCollection();
    Startup.AddDependenciesTo(containerBuilder);
    containerBuilder.AddSingleton<ExceptionThrowingFraudNotifier>();
    containerBuilder.AddSingleton<IFraudAlertNotifier>(
      context => context.GetRequiredService<ExceptionThrowingFraudNotifier>());
    containerBuilder.AddTransient<_07_ExceptionThrowingVerification>();
    containerBuilder.AddTransient<IReadOnlyCollection<IVerification>>(
      context => new List<IVerification>
      {
        context.GetRequiredService<_07_ExceptionThrowingVerification>(),
      });
    _container = containerBuilder.BuildServiceProvider();
    _verificationNotifier = _container
      .GetRequiredService<ExceptionThrowingFraudNotifier>();
    _verifier = _container.GetRequiredService<_01_CustomerVerifier>();
  }

  [TearDown]
  public void TearDown()
  {
    _container.Dispose();
  }


  /// <summary>
  /// Zakładamy, z punktu widzenia, biznesowego, że potrafimy obsłużyć
  /// sytuację rzucenia wyjątku. W naszym przypadku jest to uzyskanie
  /// wyniku przetwarzania klienta nawet jeśli wyjątek został rzucony.
  /// Nie chcemy sytuacji, w której rzucony błąd wpłynie na nasz proces
  /// biznesowy.
  ///
  /// Zakomentuj adnotację atrybut <see cref="IgnoreAttribute"/>, żeby
  /// przekonać się, że test może nie przejść (zawiesi się)!
  /// </summary>
  [Ignore("")]
  [Test]
  public async Task ShouldHandleExceptionsGracefullyWhenDealingWithResults()
  {
    var results = await _verifier.Verify(
      new Customer(Guid.NewGuid(), TooYoungStefan()),
      new CancellationToken());

    Policy.Handle<Exception>()
      .WaitAndRetryForever(_ => 100.Milliseconds())
      .Execute(() =>
      {
        results.Should().BeEquivalentTo(new [] { new VerificationResult("exception", false) });
      });
  }

  /// <summary>
  /// Poprawiamy problem z kodu wyżej. Metoda produkcyjna
  /// _01_CustomerVerifier.VerifyNoException(Customer, CancellationToken)
  /// potrafi obsłużyć rzucony wyjątek z osobnego wątku.
  /// </summary>
  [Test]
  public async Task ShouldHandleExceptionsGracefullyWhenDealingWithResultsPassing()
  {
    var results = await _verifier.VerifyNoException(
      new Customer(
        Guid.NewGuid(),
        TooYoungStefan()),
      new CancellationToken());

    Policy.Handle<Exception>()
      .WaitAndRetryForever(_ => 100.Milliseconds())
      .Execute(() =>
      {
        results.Should().BeEquivalentTo(new [] { new VerificationResult("exception", false) });
      });
  }

  /// <summary>
  /// W przypadku .Net Core, domyślnie, zadanie (task) jeśli rzuci wyjątek,
  /// to zostanie on zapisany i rzucony dopiero, gdy ktoś na takie zadanie
  /// "poczeka" (np. używając await).
  ///
  /// W tym kodzie używamy <see cref="_04_VerificationNotifier"/>,
  /// Który uruchamia zadania "w tle", nie przejmując się ich rezultatem
  /// (swoją drogą w prawdziwym kodzie warto mieć jakąkolwiek logikę obsługi
  /// wyjątków w takich "luźnych" zadaniach).
  /// 
  /// W tym teście chcemy się upewnić, że takie procesowanie nie wpływa
  /// na nasz proces biznesowy. W tym celu nadpisujemy główną implementację
  /// zamieniając ją na <see cref="ExceptionThrowingFraudNotifier"/>, który
  /// możemy zweryfikować czy miał swoją metodę wykonaną, zanim wyjątek został
  /// rzucony.
  /// </summary>
  [Test]
  public void ShouldHandleExceptionsGracefully()
  {
    _verifier.FoundFraud(new Customer(Guid.NewGuid(), TooYoungStefan()));

    Policy.Handle<AssertionException>()
      .WaitAndRetryForever(_ => 100.Milliseconds())
      .Execute(() => _verificationNotifier.Executed.Should().BeTrue());
  }

  private static Person TooYoungStefan()
  {
    return new Person("", "", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male, "0123456789");
  }
}

/// <summary>
/// Testowa implementacja weryfikacji, która będzie rzucać wyjątkiem.
/// </summary>
internal class _07_ExceptionThrowingVerification : IVerification
{
  private readonly ILogger<_07_ExceptionThrowingVerification> _logger;

  public _07_ExceptionThrowingVerification(ILogger<_07_ExceptionThrowingVerification> logger)
  {
    _logger = logger;
  }

  public string Name { get; } = "exception";

  public async Task<VerificationResult> PassesAsync(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running this in a separate thread");
    throw new InvalidOperationException("Boom!");
  }

  public VerificationResult Passes(Person person)
  {
    throw new InvalidOperationException("Boom!");
  }
}

/// <summary>
/// Testowa implementacja <see cref="IFraudAlertNotifier"/>,
/// która rzuci wyjątek po odpaleniu osobnego zadania.
/// Oczekujemy, że ten wyjątek nie zepsuje naszej logiki.
/// </summary>
internal class ExceptionThrowingFraudNotifier : IFraudAlertNotifier
{
  private readonly ILogger<ExceptionThrowingFraudNotifier> _logger;

  public ExceptionThrowingFraudNotifier(ILogger<ExceptionThrowingFraudNotifier> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// Jeśli metoda została wykonana, pole Executed zostanie ustawione
  /// na true. Następnie rzuca wyjątek.
  /// </summary>
  /// <param name="customerVerification">weryfikacja klienta</param>
  public Task FraudFound(CustomerVerification customerVerification)
  {
    return Task.Run(() =>
    {
      Executed = true;
      _logger.LogInformation("Running fraud notification in a new thread");
      throw new InvalidOperationException("Boom!");
    });
  }

  /// <summary>
  /// W C# operacje odczytu i zapisu na typie bool sa atomowe.
  /// Zobacz: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#atomicity-of-variable-references
  /// </summary>
  public bool Executed;
}