using System.Threading.Tasks;
using FraudDetection.Customers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace FraudDetection.Verifier;

/// <summary>
/// Kontroler, który dla żądania HTTP z metodą POST, w ciele którego
/// znajdzie się obiekt z klientem w postaci JSONa, zweryfikuje czy
/// dana osoba jest oszustem czy też nie.
/// </summary>
[ApiController]
[Route("[controller]")]
public class FraudController : ControllerBase
{
  private readonly ICustomerVerifier _customerVerifier;
  private readonly ILogger<FraudController> _logger;
  private readonly Counter _fraudCheckVerificationFailureCounter = Metrics.CreateCounter("fraudcheck_results_failure", "Fraud verification failure counter");
  private readonly Counter _fraudCheckVerificationSuccessCounter = Metrics.CreateCounter("fraudcheck_results_success", "Fraud verification success counter");


  public FraudController(ICustomerVerifier customerVerifier, ILogger<FraudController> logger)
  {
    _customerVerifier = customerVerifier;
    _logger = logger;
  }

  /// <summary>
  /// Metoda, która zostanie uruchomiona w momencie uzyskania odpowiedniego żądania HTTP.
  /// </summary>
  /// <param name="customer">zdeserializowany obiekt z formatu JSON</param>
  /// <returns>status 200 dla osoby uczciwej, 401 dla oszusta</returns>
  [HttpPost("fraudCheck")]
  public async Task<IActionResult> FraudCheck(Customer customer)
  {
    _logger.LogInformation($"Received a verification request for person [{customer}]");
    var result = await _customerVerifier.Verify(customer);
    if (result.Status == VerificationStatus.VerificationFailed)
    {
      _fraudCheckVerificationFailureCounter.Inc();
      return Unauthorized();
    }
    _fraudCheckVerificationSuccessCounter.Inc();
    return Ok();
  }
}