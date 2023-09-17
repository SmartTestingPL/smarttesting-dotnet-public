using System;
using FraudDetection.Customers;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier.Verification;

/// <summary>
/// Weryfikacja po wieku. Osoba w odpowiednim wieku zostanie
/// zweryfikowana pozytywnie.
/// </summary>
public class AgeVerification : IVerification
{
  private readonly ILogger<AgeVerification> _log;

  public AgeVerification(ILogger<AgeVerification> log)
  {
    _log = log;
  }

  public bool Passes(Person person)
  {
    if (person.GetAge() < 0)
    {
      throw new InvalidOperationException("Age cannot be negative.");
    }

    var passed = person.GetAge() >= 16 && person.GetAge() <= 99;
    _log.LogInformation($"Person [{person}] passed the age check [{passed}]");
    return passed;
  }
}