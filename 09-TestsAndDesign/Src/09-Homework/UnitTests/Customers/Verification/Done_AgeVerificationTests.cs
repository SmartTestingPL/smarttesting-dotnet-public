using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

/**
 * Pierwotny test ma dobry zamiary, ale wykonanie niestety takowe nie jest...
 * 
 * Zamieniając w pierwotnym teście warunki weryfikacji mocka widzimy, że test dalej przechodzi.
 * Ponadto, okazuje się, że IllegalStateException może polecieć z getAge() (co ma miejsce, jak 
 * przekazujemy nulla). 
 * 
 * Czyli powinniśmy sprawdzić wiadomość wyjątku i napisać dwa scenariusze testowe - jeden dla nulla
 * i jeden dla negatywnego wieku. 
 */
class Done_AgeVerificationTests
{

  IEventEmitter emitter;
  AgeVerification verification;

  [SetUp]
  public void SetUp()
  {
    emitter = Substitute.For<IEventEmitter>();
    verification = new AgeVerification(emitter);
  }

  [Test]
  public void ShouldThrowExceptionWhenDateOfBirthNotSet()
  {
    verification.Invoking(v =>
        v.Passes(new Person("jan", "kowalski", Maybe<LocalDate>.Nothing, Gender.Male.Just(), "abcdefghijkl")))
      .Should().ThrowExactly<InvalidOperationException>()
      .WithMessage("*Date of birth is required at this point*");
  }

  /// <summary>
  /// Ustawiając datę na przyszłość uzyskujemy wartość ujemną wieku. W ten sposób
  /// jesteśmy w stanie zweryfikować, że emitter się wykonał.
  /// </summary>
  [Test]
  public void ShouldEmitEventWhenDateOfBirthNegative()
  {
    verification.Invoking(v =>
        v.Passes(new Person("jan", "kowalski", Clocks.ZonedUtc.GetCurrentDate().PlusDays(5).Just(), Gender.Male.Just(), "abcdefghijkl")))
      .Should().ThrowExactly<InvalidOperationException>()
      .WithMessage("*Age cannot be negative*");
    emitter.Received(1).Emit(new VerificationEvent(false));
  }
}