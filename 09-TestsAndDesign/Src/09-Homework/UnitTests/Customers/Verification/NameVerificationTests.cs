using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy ten test w ogóle coś testuje?")]
public class NameVerificationTests
{
  [Test]
  public void ShouldVerifyPositivelyWhenNameIsAlphanumeric()
  {
    var verification = new NameVerification(new EventEmitter());
    var person = WithValidName();
    var expected = verification.Verify(person);

    verification.Passes(person).Should().Be(expected);
  }

  [Test]
  public void ShouldVerifyNegativelyWhenNameIsNotAlphanumeric()
  {
    var verification = new NameVerification(new EventEmitter());
    var person = WithInvalidName();
    var expected = verification.Verify(person);

    verification.Passes(person).Should().Be(expected);
  }

  private static Person WithValidName()
  {
    return new Person("jan", "kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }

  private static Person WithInvalidName()
  {
    return new Person(null, null, Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "abcdefghijk");
  }
}