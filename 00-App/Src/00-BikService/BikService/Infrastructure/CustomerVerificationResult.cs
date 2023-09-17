namespace BikService.Infrastructure;

/// <summary>
/// Rezultat weryfikacji klienta.
/// </summary>
public class CustomerVerificationResult
{
  public CustomerVerificationResult(Guid userId, Statuses status)
  {
    UserId = userId;
    Status = status;
  }

  public static CustomerVerificationResult Passed(Guid userId)
  {
    return new CustomerVerificationResult(userId, Statuses.VerificationPassed);
  }

  public static CustomerVerificationResult Failed(Guid userId)
  {
    return new CustomerVerificationResult(userId, Statuses.VerificationFailed);
  }

  public Guid UserId { get; set; }
  public Statuses Status { set; get; }

  public bool Passed()
  {
    return Statuses.VerificationPassed == Status;
  }

  public enum Statuses
  {
    VerificationPassed,
    VerificationFailed
  }

  public override string ToString()
  {
    return $"CustomerVerificationResult{{userId={UserId}, status={Status}}}";
  }
}
