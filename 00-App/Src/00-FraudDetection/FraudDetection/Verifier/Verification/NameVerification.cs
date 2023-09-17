using FraudDetection.Customers;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier.Verification;

/// <summary>
/// Weryfikacja po imieniu. Dla kobiety imię musi się kończyc na "a".
/// </summary>
public class NameVerification : IVerification
{
  private readonly ILogger<NameVerification> _log;

  public NameVerification(ILogger<NameVerification> log)
  {
    _log = log;
  }

  public bool Passes(Person person)
  {
    var passed = true;
    if (person.Gender == Gender.Female)
    {
      passed = person.Name.EndsWith("a");
    }

    _log.LogInformation($"Person [{person}] passed the name check [{passed}]");
    return passed;
  }
}