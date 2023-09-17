using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Chaos;

[ApiController]
[Route("[controller]")]
public class ChaosController : ControllerBase
{
  private readonly IMemoryCache _memoryCache;
  private readonly ILogger<ChaosController> _logger;

  public ChaosController(IMemoryCache memoryCache, ILogger<ChaosController> logger)
  {
    _memoryCache = memoryCache;
    _logger = logger;
  }

  /// <summary>
  /// Włącza wstrzykiwanie wyjątków podczas dostępu do bazy danych.
  /// </summary>
  [HttpPost("enableServiceExceptionAssault")]
  public void EnableRepositoryExceptionAssault()
  {
    _logger.LogInformation($"Setting {nameof(Assaults.EnableServiceExceptionAssault)}");
    Assaults.Config.EnableServiceExceptionAssault = true;
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

  /// <summary>
  /// Czyści pamięć podręczną
  /// </summary>
  [HttpPost("invalidateCaches")]
  public void InvalidateCaches()
  {
    _logger.LogInformation("Invalidating caches");
    ((MemoryCache)_memoryCache).Compact(1);

  }
}