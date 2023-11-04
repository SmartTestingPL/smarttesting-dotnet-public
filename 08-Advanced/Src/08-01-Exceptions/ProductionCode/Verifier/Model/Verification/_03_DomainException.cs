using System;

namespace ProductionCode.Verifier.Model.Verification;

/// <summary>
/// Wyjątek dziedzinowy.
/// </summary>
public class _03_DomainException : Exception
{
  public _03_DomainException(string message) : base(message)
  {
  }

  public _03_DomainException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

}