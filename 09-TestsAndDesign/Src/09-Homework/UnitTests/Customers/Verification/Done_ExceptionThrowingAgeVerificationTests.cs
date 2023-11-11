using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

/// <summary>
/// Pierwotny test nie weryfikował nic - asercje były niedokończone. Ponadto test był nieczytelny.
/// </summary>
public class Done_ExceptionThrowingAgeVerificationTests
{
  private ExceptionThrowingAgeVerification _verification = default!;

  [SetUp]
  public void SetUp()
  {
    _verification = new ExceptionThrowingAgeVerification();
  }

  [Test]
  public void ShouldVerifyPositivelyWhenPersonIsAnAdult()
  {
    _verification.Passes(AnAdult()).Should().BeTrue();
  }

  [Test]
  public void ShouldThrowExceptionWhenPersonIsAMinor()
  {
    _verification.Invoking(v => v.Passes(AMinor()))
      .Should().Throw<Exception>().WithMessage("*You cannot be below 18 years of age*");
  }

  private static Person AnAdult()
  {
    return new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(20)).Just(),
      Gender.Female.Just(), "34567890");
  }

  private static Person AMinor()
  {
    return new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Female.Just(), "34567890");
  }
}