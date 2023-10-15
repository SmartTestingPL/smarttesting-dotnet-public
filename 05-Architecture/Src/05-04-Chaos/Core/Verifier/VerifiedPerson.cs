using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Verifier;

/// <summary>
/// Zweryfikowana osoba
/// </summary>
public class VerifiedPerson
{
  public static VerifiedPerson CreateInstance(
    Guid userId,
    string nationalIdentificationNumber,
    VerificationStatus status)
  {
    return new VerifiedPerson
    {
      Status = status.ToString(),
      NationalIdentificationNumber = nationalIdentificationNumber,
      UserId = userId
    };
  }

  /// <summary>
  /// Id osoby
  /// </summary>
  [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
  public Guid UserId { get; set; }

  /// <summary>
  /// PESEL osoby
  /// </summary>
  private string NationalIdentificationNumber { get; set; }

  /// <summary>
  /// Status osoby
  /// </summary>
  public string Status { get; set; }
}