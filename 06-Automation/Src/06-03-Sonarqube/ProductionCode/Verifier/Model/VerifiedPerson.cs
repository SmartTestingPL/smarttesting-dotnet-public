using System;

namespace ProductionCode.Verifier.Model;

/// <summary>
/// zweryfikowana osoba.
/// </summary>
public class VerifiedPerson
{
  public static VerifiedPerson CreateInstance(
    Guid userId,
    string nationalIdentificationNumber,
    VerificationStatus status)
  {
    return new()
    {
      Status = status.ToString(),
      NationalIdentificationNumber = nationalIdentificationNumber,
      UserId = userId
    };
  }

  /// <summary>
  /// Id osoby
  /// </summary>
  public Guid UserId { get; set; }

  /// <summary>
  /// PESEL osoby
  /// </summary>
  private string? NationalIdentificationNumber { get; set; }

  /// <summary>
  /// Status osoby
  /// </summary>
  public string? Status { get; set; }
}