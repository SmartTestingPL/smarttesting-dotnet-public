using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Verifier.Infrastructure;

namespace WebApplication.Verifier.Model;

/// <summary>
/// Umożliwia sterowanie wstrzykiwaniem błędów poprzez HTTP
/// </summary>
[ApiController]
[Route("[controller]")]
public class ChaosController : ControllerBase
{
  private readonly ILogger<ChaosController> _logger;

  public ChaosController(ILogger<ChaosController> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// Włącza wstrzykiwanie wyjątków podczas dostępu do bazy danych.
  /// </summary>
  [HttpPost("enableRepositoryExceptionAssault")]
  public void EnableRepositoryExceptionAssault()
  {
    _logger.LogInformation($"Setting {nameof(Assaults.EnableExceptionAssault)}");
    Assaults.Config.EnableExceptionAssault = true;
  }

  /// <summary>
  /// Włącza wstrzykiwanie opóźnień podczas dostępu do kontrolera WebApi.
  /// </summary>
  [HttpPost("enableControllerLatencyAssault")]
  public void EnableControllerLatencyAssault(LatencyRange latencyRange)
  {
    _logger.LogInformation($"Setting {nameof(Assaults.EnableLatencyAssault)} " +
                           $"with latency range: {latencyRange.Start}-{latencyRange.End}");
    Assaults.Config.EnableLatencyAssault = true;
    Assaults.Config.LatencyRangeStart = latencyRange.Start;
    Assaults.Config.LatencyRangeEnd = latencyRange.End;
  }

  /// <summary>
  /// Przywraca ustawienia początkowe
  /// </summary>
  [HttpPost("clearAssaults")]
  public void ClearAssaults()
  {
    _logger.LogInformation("Clearing assaults");
    Assaults.Clear();
  }
}