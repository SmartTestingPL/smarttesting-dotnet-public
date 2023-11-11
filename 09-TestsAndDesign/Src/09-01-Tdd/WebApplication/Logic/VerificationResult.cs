namespace WebApplication.Logic;

public class VerificationResult
{
  public readonly VerificationStatus Status;

  public VerificationResult(VerificationStatus status)
  {
    Status = status;
  }
}