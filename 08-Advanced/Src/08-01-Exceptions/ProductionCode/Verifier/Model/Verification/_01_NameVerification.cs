using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po nazwisku.
/// </summary>
public class _01_NameVerification : IVerification
{
  public bool Passes(Person person)
  {
    Console.WriteLine($"Person's gender is [{person.GetGender()}]");
    if (person.Name == null)
    {
      throw new NullReferenceException("Name cannot be null.");
    }
    return person.Name != string.Empty;
  }
}