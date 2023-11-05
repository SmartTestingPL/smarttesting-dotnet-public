using Core.Customers;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po nazwisku.
/// </summary>
public class NameVerification : IVerification
{
  public VerificationResult Passes(Person person)
  {
    var result = !string.IsNullOrWhiteSpace(person.Name);
    return new VerificationResult("name", result);
  }
}