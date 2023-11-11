using System;

namespace ProductionCode.Verifier.Customers;

public class CustomerVerificationResult
{
  public enum Status
  {
    VerificationPassed,
    VerificationFailed
  }

  private readonly Status _status;

  private readonly Guid _userId;

  private CustomerVerificationResult(Guid userId, Status status)
  {
    _userId = userId;
    _status = status;
  }

  public static CustomerVerificationResult Passed(Guid userId)
  {
    return new CustomerVerificationResult(userId, Status.VerificationPassed);
  }

  public static CustomerVerificationResult Failed(Guid userId)
  {
    return new CustomerVerificationResult(userId, Status.VerificationFailed);
  }

  public Guid GetUserId()
  {
    return _userId;
  }

  public Status GetStatus()
  {
    return _status;
  }

  public bool Passed()
  {
    return _status == Status.VerificationPassed;
  }
}