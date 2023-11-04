using System;

namespace ProductionCode.Verifier.Model.Verification;

/// <summary>
/// Wyjątek domenowy związany z nieprawidłową weryfikacją.
/// </summary>
public class _04_VerificationException : Exception
{
  public _04_VerificationException(string message)
    : base(message)
  {
  }
}