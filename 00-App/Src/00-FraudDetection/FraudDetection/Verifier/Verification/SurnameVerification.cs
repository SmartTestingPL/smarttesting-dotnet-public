using System.Linq;
using FraudDetection.Customers;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier.Verification;

/// <summary>
/// Weryfikacja po nazwisku. Nazwisko musi mieć przynajmniej jedną samogłoskę.
/// </summary>
public class SurnameVerification : IVerification
{
  private readonly ILogger<SurnameVerification> _log;
  private static readonly string[] Vowels = { "a", "i", "o", "u" };

  public SurnameVerification(ILogger<SurnameVerification> log)
  {
    _log = log;
  }

  public bool Passes(Person person)
  {
    var passed = Vowels.Any(v => person.Surname.ToLower().Contains((string)v));
    _log.LogInformation($"Person [{person}] passed the surname check [{passed}]");
    return passed;
  }
}