using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;
using ProductionCode.Verifier.Customers;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

/// <summary>
/// Testowa implementacja komunikacji z BIK. Zwraca zawsze
/// pozytywną weryfikację (dana osoba nie jest oszustem).
/// </summary>
internal class AlwaysPassingBikVerifier : BikVerificationService
{

  public AlwaysPassingBikVerifier()
    : base("", Any.Instance<ILogger<BikVerificationService>>())
  {

  }

  public override Task<CustomerVerificationResult> Verify(Customer customer)
  {
    return Task.FromResult(CustomerVerificationResult.Passed(customer.Guid));
  }
}