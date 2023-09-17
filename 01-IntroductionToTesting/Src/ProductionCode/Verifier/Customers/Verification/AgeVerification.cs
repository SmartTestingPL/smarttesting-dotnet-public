using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja wieku osoby wnioskującej o udzielenie pożyczki.
/// </summary>
public class AgeVerification : IVerification
{
  public bool Passes(Person person)
  {
    if (person.GetAge() <= 0)
    {
      throw new InvalidOperationException("Age cannot be negative.");
    }

    return person.GetAge() >= 18 && person.GetAge() <= 99;
  }
}