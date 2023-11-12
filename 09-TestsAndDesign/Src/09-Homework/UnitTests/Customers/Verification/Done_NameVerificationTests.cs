using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

/// <summary>
/// Pierwotny test duplikuje logikę weryfikacji w sekcji given. To oznacza, że de facto nic nie testujemy.
/// Wykorzystujemy tę samą logikę do przygotowania obiektu, który oczekujemy na wyjściu. Jeśli zmieni się logika
/// biznesowa oba nasze testy dalej będą przechodzić. Szczerze mówiąc to nawet nie weryfikujemy czy wynik boolowski
/// jest true czy false - po prostu sprawdzamy czy jest taki sam jaki na wejściu.
/// </summary>
public class Done_NameVerificationTests
{
  private NameVerification _verification = default!;

  /// <summary>
  /// Możemy sobie wyciągnąć obiekt do przetestowania do pola - testy będą czytelniejsze.
  /// </summary>
  [SetUp]
  public void SetUp()
  {
    _verification = new NameVerification(new EventEmitter());
  }

  [Test]
  public void ShouldVerifyPositivelyWhenNameIsAlphanumeric()
  {
    _verification.Passes(WithValidName()).Should().BeTrue();
  }

  [Test]
  public void ShouldVerifyNegativelyWhenNameIsNotAlphanumeric()
  {
    _verification.Passes(WithInvalidName()).Should().BeFalse();
  }

  /// <summary>
  /// Często zdarza się tak, że jak obiekt w konstruktorze ma kilka parametrów tych samych typów,
  /// to można się pomylić i wstawić nie ten tam gdzie trzeba (np. name i surname). Warto jawnie 
  /// wprowadzić nieprawidłową wartość w każde inne "podejrzane" pole, tak żeby mieć pewność, że testujemy
  /// to, co powinniśmy.
  /// </summary>
  private static Person WithValidName()
  {
    return new Person("jan", null, Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }

  private static Person WithInvalidName()
  {
    return new Person(null, "Kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }
}