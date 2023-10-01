using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Encja bazodanowa. Wykorzystujemy ORM (mapowanie obiektowo relacyjne)
/// i obiekt tej klasy mapuje się na tabelę `verified`.
/// Każda własność klasy to osobna kolumna w bazie danych.
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