using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po nazwisku rzucająca wyjątek w przypadku błędu.
/// </summary>
public class _05_NameWithCustomExceptionVerification : IVerification
{
  public bool Passes(Person person)
  {
    Console.WriteLine($"Person's gender is [{person.GetGender()}]");
    if (person.Name == null)
    {
      throw new _04_VerificationException("Name cannot be null.");
    }
    return person.Name != string.Empty;
  }
}