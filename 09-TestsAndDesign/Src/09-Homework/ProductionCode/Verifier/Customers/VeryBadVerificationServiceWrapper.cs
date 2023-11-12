using System.Threading.Tasks;

namespace ProductionCode.Verifier.Customers;

public class VeryBadVerificationServiceWrapper
{
  public virtual Task<bool> Verify()
  {
    return VeryBadVerificationService
      .RunHeavyQueriesToDatabaseFromStaticMethod();
  }
}