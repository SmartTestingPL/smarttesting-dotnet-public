using System.Collections.Generic;
using System.Linq;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

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