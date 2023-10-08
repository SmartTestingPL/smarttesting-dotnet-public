using System.Linq;
using static System.Char;

namespace FraudVerifier.Customers.Verification;

public class NameVerification : IVerification
{
  public bool Passes(Person person)
  {
    return person.Name.All(IsLetter);
  }
}