using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Client;

namespace WebApplication.Controllers;

/// <summary>
/// Klasa wskazana na slajdzie opisującym kontroler FraudController.
/// Kontroler, który dla żądania HTTP z metodą POST, w ciele którego
/// znajdzie się obiekt z klientem w postaci JSONa, zweryfikuje czy dana
/// osoba jest oszustem czy też nie.
/// </summary>
[ApiController]
[Route("[controller]")]
public class FraudController : ControllerBase
{
  private readonly ICustomerVerifier _customerVerifier;
  private readonly ILogger<FraudController> _logger;

  public FraudController(ICustomerVerifier customerVerifier, ILogger<FraudController> logger)
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
  public IActionResult FraudCheck(Person person)
  {
    _logger.LogInformation($"Received a verification request for person [{person}]");
    var result = _customerVerifier.Verify(person);
    if (result.Status == VerificationStatus.VerificationFailed)
    {
      return Unauthorized();
    }
    return Ok();
  }
}