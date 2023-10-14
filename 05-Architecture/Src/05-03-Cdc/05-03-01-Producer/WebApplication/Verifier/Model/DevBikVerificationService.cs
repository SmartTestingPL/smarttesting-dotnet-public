using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier;

namespace WebApplication.Verifier.Model;

/// <summary>
/// W wersji Javowej ta klasa była anonimowa.
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