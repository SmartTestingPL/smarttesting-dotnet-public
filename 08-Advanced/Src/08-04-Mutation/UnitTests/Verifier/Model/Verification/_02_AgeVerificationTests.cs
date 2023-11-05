using System;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model.Verification;
using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NUnit.Framework;

namespace UnitTests.Verifier.Model.Verification;

public class _02_AgeVerificationTests
{
  [Test]
  public void ShouldThrowExceptionWhenAgeInvalid()
  {
    var verification = new _01_AgeVerification(NullLogger<_01_AgeVerification>.Instance);

    verification.Invoking(v => v.Passes(ZbigniewFromTheFuture()))
      .Should().Throw<InvalidOperationException>()
      .WithMessage("*Age cannot be negative*");
  }

  private static Person ZbigniewFromTheFuture()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Plus(Period.FromYears(10)).Just(), 
      Gender.Male, 
      "1234567890");
  }

  [Test]
  public void ShouldReturnPositiveVerificationWhenAgeIsWithinTheThreshold()
  {
    var verification = new _01_AgeVerification(
      NullLogger<_01_AgeVerification>.Instance);

    var result = verification.Passes(OldEnoughZbigniew());

    result.Result.Should().BeTrue();
  }

  private static Person OldEnoughZbigniew()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski", 
      Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(25)).Just(), 
      Gender.Male, 
      "1234567890");
  }

  [Test]
  public void ShouldReturnNegativeVerificationWhenAgeIsBelowTheThreshold()
  {
    var verification = new _01_AgeVerification(
      NullLogger<_01_AgeVerification>.Instance);

    var result = verification.Passes(TooYoungZbigniew());

    result.Result.Should().BeFalse();
  }

  private static Person TooYoungZbigniew()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(), 
      Gender.Male, 
      "1234567890");
  }

  [Test]
  public void ShouldReturnNegativeVerificationWhenAgeIsAboveTheThreshold()
  {
    var verification = new _01_AgeVerification(NullLogger<_01_AgeVerification>.Instance);

    var result = verification.Passes(TooOldZbigniew());

    result.Result.Should().BeFalse();
  }

  private static Person TooOldZbigniew()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(1000)).Just(), 
      Gender.Male, "1234567890");
  }

  ///<summary>
  ///Zakomentuj <see cref="IgnoreAttribute"/>, żeby zwiększyć pokrycie kodu
  /// testami. Pokrywamy warunki brzegowe!
  ///</summary>
  [Ignore("")]
  [Test]
  public void ShouldReturnNegativeVerificationWhenAgeIsInLowerBoundary()
  {
    var verification = new _01_AgeVerification(
      NullLogger<_01_AgeVerification>.Instance);

    var result = verification.Passes(LowerAgeBoundaryZbigniew());

    result.Result.Should().BeTrue();
  }

  private static Person LowerAgeBoundaryZbigniew()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(18)).Just(), 
      Gender.Male, "1234567890");
  }

  ///<summary>
  ///Zakomentuj <see cref="IgnoreAttribute"/>, żeby zwiększyć pokrycie kodu
  /// testami. Pokrywamy warunki brzegowe!
  ///</summary>
  [Ignore("")]
  [Test]
  public void ShouldReturnNegativeVerificationWhenAgeIsInUpperBoundary()
  {
    var verification = new _01_AgeVerification(
      NullLogger<_01_AgeVerification>.Instance);

    var result = verification.Passes(UpperAgeBoundaryZbigniew());

    result.Result.Should().BeTrue();
  }

  private static Person UpperAgeBoundaryZbigniew()
  {
    return new Person(
      "Zbigniew", 
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(99)).Just(), 
      Gender.Male, 
      "1234567890");
  }
}