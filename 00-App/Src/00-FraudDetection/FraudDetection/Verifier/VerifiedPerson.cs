using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FraudDetection.Verifier;

/// <summary>
/// Encja bazodanowa. Wykorzystujemy ORM (mapowanie obiektowo relacyjne)
/// i obiekt tej klasy mapuje się na tabelę `verified`. Każde pole
/// klasy to osobna kolumna w bazie danych.
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
  public string NationalIdentificationNumber { get; set; }
  public string Status { get; set; }
}