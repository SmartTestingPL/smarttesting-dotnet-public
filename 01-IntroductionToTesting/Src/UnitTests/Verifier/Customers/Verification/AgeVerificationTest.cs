using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Verifier.Customers.Verification;

/// <summary>
///Klasa zawiera przykłady wykorzystania bibliotek do asercji.
/// Tu akurat używam biblioteki FluentAssertions, która jest
/// odpowiednikiem AssertJ używanego w wersji Javowej
/// (jeśli tylko zignorujemy marketingowe hasła typu "BDD Assertions").
/// </summary>
public class AgeVerificationTest
{
  [Test]
  public void VerificationShouldPassForAgeBetween18And99()
  {
    // given
    var person = PersonWhoHasLivedFor(22.Years());
    var verification = new AgeVerification();

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeTrue();
  }

  [Test]
  public void ShouldReturnFalseWhenUserOlderThan99()
  {
    // given
    var person = PersonWhoHasLivedFor(100.Years());
    var verification = new AgeVerification();

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  // Weryfikacja wyjątku przy pomocy biblioteki do asercji.
  // NUnit ma też wbudowane metody służące do tego, takie jak 
  // Assert.Throws, Assert.ThrowsAsync,
  // Assert.Catch, Assert.CatchAsync
  [Test]
  public void TestIllegalStateExceptionThrownWhenAgeBelowZero()
  {
    // given
    var person = PersonWhoHasLivedFor((-1).Years());
    var verification = new AgeVerification();

    verification.Invoking(v => v.Passes(person))
      .Should().Throw<InvalidOperationException>();
  }

  /// <summary>
  /// Metoda pomocnicza tworząca obiekty wykorzystywane w testach,
  /// używana w celu uzyskania lepszej czytelności kodu i ponownego
  /// użycia kodu.
  /// </summary>
  /// <param name="age">wiek</param>
  private static Person PersonWhoHasLivedFor(Period age)
  {
    var birthDate = Clocks.ZonedUtc.GetCurrentDate() - age;
    return new Person("Anna", "Smith", birthDate.Just(),
      Gender.Female, "00000000000");
  }
}