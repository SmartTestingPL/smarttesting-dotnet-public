using System.Collections.Generic;
using System.Linq;

namespace FraudVerifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(IReadOnlyCollection<IVerification> verifications)
  {
    _verifications = verifications;
  }

  public CustomerVerificationResult Verify(Customer customer)
  {
    if (_verifications.All(verification => verification.Passes(customer.Person)))
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}