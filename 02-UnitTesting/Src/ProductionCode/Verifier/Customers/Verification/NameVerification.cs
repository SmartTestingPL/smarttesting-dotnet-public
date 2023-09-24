using System.Linq;
using ProductionCode.Customers;
using static System.Char;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja po nazwisku.
/// </summary>
public class NameVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;

  public NameVerification(IEventEmitter eventEmitter)
  {
    _eventEmitter = eventEmitter;
  }

  public bool Passes(Person person)
  {
    var passes = person.Name.All(IsLetter);
    _eventEmitter.Emit(new VerificationEvent(passes));
    return passes;
  }
}