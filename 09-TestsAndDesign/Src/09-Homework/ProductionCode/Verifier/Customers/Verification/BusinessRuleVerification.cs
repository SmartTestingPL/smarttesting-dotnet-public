using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja po warunkach biznesowych. Chyba ta klasa robi za dużo, no
/// ale trudno...
/// </summary>
public class BusinessRulesVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;
  private readonly VerifierManagerImpl _verifier;

  public BusinessRulesVerification(IEventEmitter eventEmitter, VerifierManagerImpl verifier)
  {
    _eventEmitter = eventEmitter;
    _verifier = verifier;
  }

  public bool Passes(Person person)
  {
    var passes = _verifier.VerifyName(person);
    passes = passes && _verifier.VerifyAddress(person);
    passes = passes && _verifier.VerifyPhone(person);
    passes = passes && _verifier.VerifySurname(person);
    passes = passes && _verifier.VerifyTaxInformation(person);
    _eventEmitter.Emit(new VerificationEvent(passes));
    return passes;
  }
}