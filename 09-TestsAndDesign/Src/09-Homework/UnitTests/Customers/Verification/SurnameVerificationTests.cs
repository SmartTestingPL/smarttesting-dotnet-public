using FluentAssertions;
using Core.Maybe;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy framework do mockowania działa?")]
public class SurnameVerificationTests
{
  private ISurnameChecker _checker = default!;
  private SurnameVerification _verification = default!;

  [SetUp]
  public void SetUp()
  {
    _checker = Substitute.For<ISurnameChecker>();
    _verification = new SurnameVerification(_checker);
  }

  [Test]
  public void ShouldReturnFalseWhenSurnameInvalid()
  {
    _checker.CheckSurname(Arg.Any<Person>()).Returns(false);

    _verification.Passes(Person()).Should().BeFalse();
  }

  [Test]
  public void ShouldReturnTrueWhenSurnameInvalid()
  {
    _checker.CheckSurname(Arg.Any<Person>()).Returns(true);

    _verification.Passes(Person()).Should().BeTrue();
  }

  private static Person Person()
  {
    return new Person("a", "b", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "1234567890");
  }

}