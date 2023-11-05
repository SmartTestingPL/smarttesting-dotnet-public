using Core.Customers;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po PESELu.
/// </summary>
public class IdentificationNumberVerification : IVerification
{
  public VerificationResult Passes(Person person)
  {
    var result = GenderMatchesIdentificationNumber(person);
    return new VerificationResult("id", result);
  }

  private static bool GenderMatchesIdentificationNumber(Person person)
  {
    if (int.Parse(person.NationalIdentificationNumber.Substring(9, 1)) % 2 == 0)
    {
      return person.Gender == Gender.Female;
    }

    return person.Gender == Gender.Male;
  }
}