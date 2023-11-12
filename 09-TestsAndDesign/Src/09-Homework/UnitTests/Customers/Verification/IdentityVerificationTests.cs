using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy ten test weryfikuje kod produkcyjny?")]
public class IdentityVerificationTests
{
  [Test]
  public void ShouldVerifyNegativelyForAnInvalidIdentityNumber()
  {
    var verification = new IdentityVerification();

    verification.Passes(WithInvalidPesel()).Should().BeFalse();
  }

  private static Person WithInvalidPesel()
  {
    return new Person("jan", "kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }
}