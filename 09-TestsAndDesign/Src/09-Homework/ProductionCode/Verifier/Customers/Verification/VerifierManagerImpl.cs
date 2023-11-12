using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Klasa udająca klasę, która robi zdecydowanie za dużo.
/// </summary>
public class VerifierManagerImpl
{
  public virtual bool VerifyTaxInformation(Person person)
  {
    return true;
  }

  public virtual bool VerifyAddress(Person person)
  {
    return true;
  }

  public virtual bool VerifyName(Person person)
  {
    return true;
  }

  public virtual bool VerifySurname(Person person)
  {
    return true;
  }

  public virtual bool VerifyPhone(Person person)
  {
    return true;
  }
}