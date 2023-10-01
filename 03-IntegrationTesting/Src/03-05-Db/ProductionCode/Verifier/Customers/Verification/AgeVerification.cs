using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja po wieku. Osoba w odpowiednim wieku zostanie
/// zweryfikowana pozytywnie.
/// </summary>
public class AgeVerification : IVerification
{
  public bool Passes(Person person)
  {
    if (person.GetAge() < 0)
    {
      throw new InvalidOperationException("Age cannot be negative.");
    }

    var passes = person.GetAge() >= 18 && person.GetAge() <= 99;
    return passes;
  }
}