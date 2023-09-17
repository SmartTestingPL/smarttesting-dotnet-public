using System;
using System.Threading.Tasks;
using FraudDetection.Chaos;
using FraudDetection.Customers;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace FraudDetection.Verifier;

public class CustomerVerifierWatcher : ICustomerVerifier
{
  private readonly AsyncInjectOutcomePolicy _chaosPolicy = MonkeyPolicy.InjectExceptionAsync(with =>
    with.Fault(new Exception("thrown from exception attack!"))
      .InjectionRate(1)
      .EnabledWhen((context, token) => 
        Task.FromResult(Assaults.Config.EnableServiceExceptionAssault))
  );
  private readonly CustomerVerifier _verifier;

  public CustomerVerifierWatcher(CustomerVerifier verifier)
  {
    _verifier = verifier;
  }

  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    var capturedResult = await _chaosPolicy.ExecuteAndCaptureAsync(
      async () => await _verifier.Verify(customer));
    if (capturedResult.Outcome == OutcomeType.Failure)
    {
      throw capturedResult.FinalException;
    }
    else
    {
      return capturedResult.Result;
    }
  }
}