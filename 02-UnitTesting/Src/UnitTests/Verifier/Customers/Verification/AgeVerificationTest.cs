using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;
using static TddXt.AnyRoot.Root;

namespace UnitTests.Verifier.Customers.Verification;

/// <summary>
/// Klasa zawiera przykłady asercji z wykorzystaniem bibliotek do asercji.
/// </summary>
public class AgeVerificationTest
{
  [Test]
  public void VerificationShouldPassForAgeBetween18And99()
  {
    // given
    var person = PersonWhoHasLivedFor(22.Years());
    var verification = new AgeVerification(Any.Instance<IEventEmitter>());

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
    var verification = new AgeVerification(Any.Instance<IEventEmitter>());

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  // Weryfikacja wyjątku przy pomocy biblioteki do asercji.
  [Test]
  public void TestIllegalStateExceptionThrownWhenAgeBelowZero()
  {
    // given
    var person = PersonWhoHasLivedFor((-1).Years());
    var verification = new AgeVerification(Any.Instance<IEventEmitter>());

    verification.Invoking(v => v.Passes(person))
      .Should().Throw<InvalidOperationException>();
  }

  private static Person PersonWhoHasLivedFor(Period age)
  {
    var birthDate = Clocks.ZonedUtc.GetCurrentDate() - age;
    return new Person("Anna", "Smith", birthDate.Just(),
      Gender.Female, "00000000000");
  }
}