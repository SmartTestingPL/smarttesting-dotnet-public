using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

public interface ISurnameChecker
{
  bool CheckSurname(Person person);
}

/// <summary>
/// Klasa udająca, że weryfikuje osobę po nazwisku.
/// </summary>
public class SurnameChecker : ISurnameChecker
{
  public bool CheckSurname(Person person)
  {
    return false;
  }
}