using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers;

/// <summary>
/// Kontroler, który dla żądania HTTP z metodą POST, w ciele którego
/// znajdzie się obiekt z klientem w postaci JSONa, zweryfikuje czy dana
/// osoba jest oszustem czy też nie.
/// </summary>
[Route("[controller]")]
[ApiController]
public class FraudController : ControllerBase
{
  private readonly ICustomerVerifier _customerVerifier;
  private readonly ILogger<FraudController> _logger;

  public FraudController(
    ICustomerVerifier customerVerifier,
    ILogger<FraudController> logger)
  {
    _customerVerifier = customerVerifier;
    _logger = logger;
  }

  /// <summary>
  /// Metoda, która zostanie uruchomiona w momencie uzyskania odpowiedniego
  /// żądania HTTP.
  /// </summary>
  /// <param name="customer">zdeserializowany obiekt z formatu JSON</param>
  /// <returns>status 200 dla osoby uczciwej, 401 dla oszusta</returns>
  [HttpPost("fraudCheck")]
  public async Task<IActionResult> FraudAsync(
    Customer customer, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Received a verification request " +
                           $"for pl.smarttesting.customer [{customer}]");
    var result = (await _customerVerifier.Verify(customer, cancellationToken))
      .All(r => r.Result)
        ? CustomerVerificationResult.Passed(customer.Guid)
        : CustomerVerificationResult.Failed(customer.Guid);

    if (result.Status == VerificationStatus.VerificationFailed)
    {
      return Unauthorized();
    }

    return Ok();
  }
}