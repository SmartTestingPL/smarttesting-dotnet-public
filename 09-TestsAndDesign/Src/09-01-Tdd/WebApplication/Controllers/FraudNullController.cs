using Microsoft.AspNetCore.Mvc;
using WebApplication.Logic;

namespace WebApplication.Controllers;

[Route("fraudNull")]
[ApiController]
public class FraudNullController : ControllerBase
{
  [HttpPost("fraudCheck")]
  public VerificationResult Fraud(Client client)
  {
    return null;
  }
}