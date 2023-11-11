using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja wieku osoby wnioskującej o udzielenie pożyczki.
/// </summary>
public class SurnameVerification : IVerification
{
  private readonly ISurnameChecker _surnameChecker;

  public SurnameVerification(ISurnameChecker surnameChecker)
  {
    _surnameChecker = surnameChecker;
  }

  public bool Passes(Person person)
  {
    return _surnameChecker.CheckSurname(person);
  }
}