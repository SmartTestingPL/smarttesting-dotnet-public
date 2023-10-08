using FraudVerifier.Customers;

namespace FraudVerifier;

/// <summary>
/// Weryfikacja klienta.
/// </summary>
public interface IVerification
{
  bool Passes(Person person);
}