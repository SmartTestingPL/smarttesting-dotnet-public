using System.Linq;
using ProductionCode.Customers;
using static System.Char;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja po imieniu.
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
    var passes = Verify(person);
    _eventEmitter.Emit(new VerificationEvent(passes));
    return passes;
  }

  public bool Verify(Person person)
  {
    return person.Name != null && person.Name.All(IsLetter);
  }
}