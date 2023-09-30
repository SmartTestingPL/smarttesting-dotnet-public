using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Verifier.Customers;

/// <summary>
/// Encja bazodanowa. Wykorzystujemy ORM
/// (mapowanie obiektowo relacyjne) i obiekt
/// tej klasy mapuje się na wpis w mapowaniu
/// klucz wartość o kluczu `verified`.
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

  [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
  public Guid UserId { get; set; }
  private string NationalIdentificationNumber { get; set; }
  public string Status { get; set; }
}