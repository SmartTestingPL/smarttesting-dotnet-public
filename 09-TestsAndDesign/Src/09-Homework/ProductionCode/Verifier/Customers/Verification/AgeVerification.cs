using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

public class AgeVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;

  public AgeVerification(IEventEmitter eventEmitter)
  {
    _eventEmitter = eventEmitter;
  }

  public bool Passes(Person person)
  {
    if (person.GetAge() <= 0)
    {
      _eventEmitter.Emit(new VerificationEvent(false));
      throw new InvalidOperationException("Age cannot be negative.");
    }

    var passes = person.GetAge() >= 18 && person.GetAge() <= 99;
    _eventEmitter.Emit(new VerificationEvent(passes));
    return passes;
  }
}