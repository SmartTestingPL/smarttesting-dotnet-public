using ProductionCode.Customers;

namespace ProductionCode.Verifier;

public interface IVerification
{
  bool Passes(Person person);
}