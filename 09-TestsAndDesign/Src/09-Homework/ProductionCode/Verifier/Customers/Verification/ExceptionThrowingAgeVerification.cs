using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

///<summary>
///Weryfikacja po wieku - jeśli nie przechodzi to leci wyjątek.
///</summary>
public class ExceptionThrowingAgeVerification : IVerification
{
  public bool Passes(Person person)
  {
    var passes = person.GetAge() >= 18;
    if (!passes)
    {
      throw new ArgumentException("You cannot be below 18 years of age!", nameof(person));
    }

    return passes;
  }
}