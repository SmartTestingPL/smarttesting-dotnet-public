using FraudDetection.Customers;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier.Verification;

/// <summary>
/// Weryfikacja po płci. Płeć musi być wybrana.
/// </summary>
public class GenderVerification : IVerification
{
  private readonly ILogger<GenderVerification> _log;

  public GenderVerification(ILogger<GenderVerification> log)
  {
    _log = log;
  }

  public bool Passes(Person person)
  {
    var passed = person.Gender != null;
    _log.LogInformation($"Person [{person}] passed the gender check [{passed}]");
    return passed;
  }
}