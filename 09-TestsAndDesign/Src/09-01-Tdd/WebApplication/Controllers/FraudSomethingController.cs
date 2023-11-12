using Microsoft.AspNetCore.Mvc;
using WebApplication.Logic;

namespace WebApplication.Controllers;

[Route("fraudSomething")]
[ApiController]
public class FraudSomethingController : ControllerBase
{
  private readonly Something _something;

  public FraudSomethingController(Something something)
  {
    _something = something;
  }

  [HttpPost("fraudCheck")]
  public VerificationResult Fraud(Client client)
  {
    return _something.GetSomething(client);
  }
}