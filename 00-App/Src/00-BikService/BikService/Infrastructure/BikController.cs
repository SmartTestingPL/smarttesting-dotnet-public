using System.Net;
using System.Net.Mime;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using Microsoft.AspNetCore.Mvc;

namespace BikService.Infrastructure;

[ApiController]
public class BikController : ControllerBase
{
  private readonly IScoreAnalyzer _scoreAnalyzer;
  private readonly ILogger<BikController> _log;

  public BikController(IScoreAnalyzer scoreAnalyzer, ILogger<BikController> log)
  {
    _scoreAnalyzer = scoreAnalyzer;
    _log = log;
  }

  [HttpGet("/{requestedPesel}")]
  [Produces(MediaTypeNames.Application.Json)]
  public async Task<ActionResult<CustomerVerificationResult>> Score(string requestedPesel)
  {
    var pesel = new Pesel(requestedPesel);
    if (await _scoreAnalyzer.ShouldGrantLoan(pesel))
    {
      var passed = CustomerVerificationResult.Passed(Guid.NewGuid());
      _log.LogInformation("Passed. Returning " + passed);
      return new OkObjectResult(passed);
    }

    var failed = CustomerVerificationResult.Failed(Guid.NewGuid());
    _log.LogInformation("Failed. Returning " + failed);
    return new ObjectResult(failed)
    {
      StatusCode = (int)HttpStatusCode.Forbidden
    };
  }
}