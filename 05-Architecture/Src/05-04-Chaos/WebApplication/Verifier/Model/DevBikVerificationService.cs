using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier;

namespace WebApplication.Verifier.Model;

/// <summary>
/// W oryginale Javowym jest to klasa anonimowa.
/// Jako że C# nie wspiera klas anonimowych w rozumieniu
/// Javowym, musiałem ją zadeklarować jako pełnoprawną klasę.
///
/// Fałszywa implementacja usługi weryfikacyjnej.
/// </summary>
public class DevBikVerificationService : IBikVerificationService
{
  public Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
  {
    if (customer.Person.Name == "Fraudeusz")
    {
      return Task.FromResult(CustomerVerificationResult.Failed(customer.Guid));
    }

    return Task.FromResult(CustomerVerificationResult.Passed(customer.Guid));
  }
}