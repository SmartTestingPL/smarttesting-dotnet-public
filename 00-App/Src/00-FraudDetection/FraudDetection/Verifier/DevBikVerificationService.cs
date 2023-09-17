using System.Threading.Tasks;
using FraudDetection.Customers;

namespace FraudDetection.Verifier;

/// <summary>
/// W oryginale Javowym jest to klasa anonimowa.
/// Jako że C# nie wspiera klas anonimowych w rozumieniu
/// Javowym, musiałem ją zadeklarować jako pełnoprawną klasę.
/// </summary>
public class DevBikVerificationService : IBikVerificationService
{
  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    if (customer.Person.Name == "Fraudeusz")
    {
      return await Task.FromResult(CustomerVerificationResult.Failed(customer.Guid));
    }

    return await Task.FromResult(CustomerVerificationResult.Passed(customer.Guid));
  }
}