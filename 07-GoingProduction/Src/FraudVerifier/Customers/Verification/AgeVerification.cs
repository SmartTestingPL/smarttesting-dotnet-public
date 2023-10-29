using System;

namespace FraudVerifier.Customers.Verification;

/// <summary>
/// Weryfikacja po wieku.
/// </summary>
public class AgeVerification : IVerification
{
  public bool Passes(Person person)
  {
    if (person.GetAge() < 0)
    {
      throw new InvalidOperationException("Age cannot be negative.");
    }

    return person.GetAge() >= 18 && person.GetAge() <= 99;
  }
}