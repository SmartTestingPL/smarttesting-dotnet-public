using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

/// <summary>
/// Pierwotny test testuje tylko negatywny przypadek. Kod produkcyjny zawiera automatycznie generowany kod
/// przez IDE. Czasami zdarza się, że test przechodzi, a nie powinien tylko dlatego, że użyte
/// zostały wartości domyślne takie jak null, 0, false.
/// </summary>
public class Done_IdentityVerificationTests
{

  /// <summary>
  /// Test waliduje weryfikacje osoby z błędnym numerem PESEL. 
  /// </summary>
  [Test]
  public void ShouldVerifyNegativelyForAnInvalidIdentityNumber()
  {
    var verification = new IdentityVerification();

    verification.Passes(WithInvalidPesel()).Should().BeFalse();
  }

  /// <summary>
  /// Test waliduje weryfikacje osoby z poprawnym numerem PESEL.
  /// Wywali się, ponieważ kod produkcyjny zawiera tylko wartości domyślne. 
  /// </summary>
  [Test]
  [Ignore("Wywali się, bo kod produkcyjny zawiera tylko wartości domyślne")]
  public void ShouldVerifyPositivelyForAValidIdentityNumber()
  {
    var verification = new IdentityVerification();

    verification.Passes(WithValidPesel()).Should().BeTrue();
  }

  private static Person WithInvalidPesel()
  {
    return new Person("jan", "kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }

  private static Person WithValidPesel()
  {
    return new Person("jan", "kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "49120966834");
  }
}