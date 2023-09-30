using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;
using ProductionCode.Verifier.Customers;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

///<summary>
/// Testowa implementacja komunikacji z BIK. Rzuca wyjątkiem
/// jeśli zostanie wywołana. W ten sposób upewniamy się, że test
/// się wysypie jeśli spróbujemy zawołać BIK.
/// </summary>
internal class ExceptionThrowingBikVerifier : BikVerificationService
{
  public ExceptionThrowingBikVerifier()
    : base("", Any.Instance<ILogger<BikVerificationService>>())
  {
  }

  public override Task<CustomerVerificationResult> Verify(Customer customer)
  {
    throw new InvalidOperationException("Shouldn't call bik verification");
  }
}