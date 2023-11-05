using System;
using Core.Customers;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po wieku. Osoba w odpowiednim wieku zostanie
/// zweryfikowana pozytywnie.
/// </summary>
public class _01_AgeVerification : IVerification
{
  private readonly ILogger<_01_AgeVerification> _logger;

  public _01_AgeVerification(ILogger<_01_AgeVerification> logger)
  {
    _logger = logger;
  }

  public VerificationResult Passes(Person person)
  {
    if (person.GetAge() < 0)
    {
      _logger.LogWarning("Age is negative");
      throw new InvalidOperationException("Age cannot be negative.");
    }

    _logger.LogInformation($"Person has age [{person.GetAge()}]");
    var result = person.GetAge() >= 18 && person.GetAge() <= 99;
    return new VerificationResult("age", result);
  }
}