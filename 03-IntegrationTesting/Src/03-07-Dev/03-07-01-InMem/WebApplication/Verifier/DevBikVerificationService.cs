using System.Threading.Tasks;
using WebApplication.Customers;

namespace WebApplication.Verifier;

/// <summary>
/// W oryginale Javowym jest to klasa anonimowa.
/// Jako że C# nie wspiera klas anonimowych w rozumieniu
/// Javowym, musiałem ją zadeklarować jako pełnoprawną klasę.
/// </summary>
public class DevBikVerificationService : IBikVerificationService
{
  public Task<CustomerVerificationResult> Verify(Customer customer)
  {
    if (customer.Person.Name == "Fraudeusz")
    {
      return Task.FromResult(CustomerVerificationResult.Failed(customer.Guid));
    }

    return Task.FromResult(CustomerVerificationResult.Passed(customer.Guid));
  }
}