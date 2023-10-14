using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication.Verifier.Model;

/// <summary>
/// Kontroler, który dla żądania HTTP z metodą POST, w ciele którego
/// znajdzie się obiekt z klientem w postaci JSONa, zweryfikuje czy dana
/// osoba jest oszustem czy też nie.
/// </summary>
[ApiController]
[Route("[controller]")]
public class FraudController : ControllerBase
{
  private readonly CustomerVerifier _customerVerifier;
  private readonly ILogger<FraudController> _logger;

  public FraudController(CustomerVerifier customerVerifier, ILogger<FraudController> logger)
  {
    _customerVerifier = customerVerifier;
    _logger = logger;
  }

  /// <summary>
  /// Metoda, która zostanie uruchomiona w momencie uzyskania odpowiedniego żądania HTTP.
  /// </summary>
  /// <param name="person">zdeserializowany obiekt z formatu JSON</param>
  /// <returns>status 200 dla osoby uczciwej, 401 dla oszusta</returns>
  [HttpPost("fraudCheck")]
  public async Task<IActionResult> FraudCheck(Customer person)
  {
    _logger.LogInformation($"Received a verification request for person [{person}]");
    var result = await _customerVerifier.Verify(person, new CancellationToken());
    if (result.Status == VerificationStatus.VerificationFailed)
    {
      return Unauthorized();
    }
    return Ok();
  }
}