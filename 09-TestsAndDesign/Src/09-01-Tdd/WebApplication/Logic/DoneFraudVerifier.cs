using WebApplication.Controllers;

namespace WebApplication.Logic;

public class DoneFraudVerifier
{
  public VerificationResult Verify(Client client)
  {
    if (client.HasDebt)
    {
      return new VerificationResult(VerificationStatus.Fraud);
    }

    return new VerificationResult(VerificationStatus.NotFraud);
  }
}