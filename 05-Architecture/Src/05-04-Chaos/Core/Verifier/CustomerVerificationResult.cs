using System;

namespace Core.Verifier;

/// <summary>
/// Rezultat weryfikacji klienta.
/// </summary>
public class CustomerVerificationResult
{
  public CustomerVerificationResult(Guid userId, VerificationStatus status)
  {
    UserId = userId;
    Status = status;
  }

  public static CustomerVerificationResult Passed(Guid userId)
  {
    return new CustomerVerificationResult(userId, VerificationStatus.VerificationPassed);
  }

  public static CustomerVerificationResult Failed(Guid userId)
  {
    return new CustomerVerificationResult(userId, VerificationStatus.VerificationFailed);
  }

  public Guid UserId { get; }

  public VerificationStatus Status { get; }

  public bool Passed()
  {
    return Status == VerificationStatus.VerificationPassed;
  }

  public override string ToString()
  {
    return $"CustomerVerificationResult{{userId={UserId}, status={Status}}}";
  }
}

public enum VerificationStatus
{
  VerificationPassed,
  VerificationFailed
}