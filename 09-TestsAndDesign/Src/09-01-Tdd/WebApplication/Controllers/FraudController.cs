using Microsoft.AspNetCore.Mvc;
using WebApplication.Logic;

namespace WebApplication.Controllers;

[Route("[controller]")]
[ApiController]
public class FraudController : ControllerBase
{
  private readonly DoneFraudVerifier _fraudVerifier;

  public FraudController(DoneFraudVerifier fraudVerifier)
  {
    _fraudVerifier = fraudVerifier;
  }

  [HttpPost("fraudCheck")]
  public VerificationResult Fraud(Client client)
  {
    return _fraudVerifier.Verify(client);
  }
}