using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy ten test na pewno weryfikuje... cokolwiek?")]
public class AgeVerificationTests
{

  [Test]
  public void ShouldEmitEventWhenDateOfBirthInvalid()
  {
    var emitter = Substitute.For<IEventEmitter>();
    var verification = new AgeVerification(emitter);

    verification.Invoking(v =>
    {
      v.Passes(new Person("jan", "kowalski", Maybe<LocalDate>.Nothing, Gender.Male.Just(), "abcdefghijkl"));
      emitter.Received(1).Emit(new VerificationEvent(false));
    }).Should().ThrowExactly<InvalidOperationException>();
  }
}