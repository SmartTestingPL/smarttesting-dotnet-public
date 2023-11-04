using System.Collections.Generic;
using Value;

namespace Core.Verifier.Model;

/// <summary>
/// Rezultat weryfikacji klienta.
/// </summary>
public class VerificationResult : ValueType<VerificationResult>
{
  public readonly string VerificationName;
  public readonly bool Result;

  public VerificationResult(string verificationName, bool result)
  {
    VerificationName = verificationName;
    Result = result;
  }

  protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
  {
    yield return VerificationName;
    yield return Result;
  }
}