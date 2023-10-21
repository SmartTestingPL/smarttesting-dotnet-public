using System;

namespace ProductionCode.Verifier.Model;

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
    return new(userId, VerificationStatus.VerificationPassed);
  }

  public static CustomerVerificationResult Failed(Guid userId)
  {
    return new(userId, VerificationStatus.VerificationFailed);
  }

  public Guid UserId { get; }

  public VerificationStatus Status { get; }

  public bool Passed()
  {
    return Status == VerificationStatus.VerificationPassed;
  }
}

public enum VerificationStatus
{
  VerificationPassed,
  VerificationFailed
}