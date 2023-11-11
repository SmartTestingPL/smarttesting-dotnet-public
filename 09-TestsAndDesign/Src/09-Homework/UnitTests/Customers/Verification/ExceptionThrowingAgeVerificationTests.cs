using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy na pewno te asercje są poprawne?")]
public class ExceptionThrowingAgeVerificationTests
{
  private ExceptionThrowingAgeVerification _verification = default!;

  [SetUp]
  public void SetUp()
  {
    _verification = new ExceptionThrowingAgeVerification();
  }

  [Test]
  public void TestGood()
  {
    _verification.Passes(GoodPerson()).Should();
  }

  [Test]
  public void TestBad()
  {
    _verification.Invoking(v => v.Passes(BadPerson())).Should();
  }

  private static Person GoodPerson()
  {
    return new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Minus(Period.FromYears(20)).Just(),
      Gender.Female.Just(), "34567890");
  }

  private static Person BadPerson()
  {
    return new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Female.Just(), "34567890");
  }
}