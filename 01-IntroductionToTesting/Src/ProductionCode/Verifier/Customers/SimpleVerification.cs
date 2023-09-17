using System;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Implementacja weryfikacji obrazującej problemy z testami,
/// które nie weryfikują poprawnie implementacji.
/// </summary>
public class SimpleVerification : IVerification
{
  private readonly bool _verificationPassed = true;

  /// <summary>
  /// Przykład problemu w testach:
  /// metoda z niezaimplementowaną logiką, dla której przechodzą źle napisane testy
  /// </summary>
  public bool Passes(Person person)
  {
    // TODO use SomeLogicResolvingToBool(person);
    return false;
  }

  private bool SomeLogicResolvingToBool(Person person)
  {
    // TODO: calculate based on verificationPassed value
    throw new NotImplementedException("Not yet implemented!");
  }

  public bool VerificationPasses()
  {
    return _verificationPassed;
  }
}