using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BikService.Chaos;

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
  [HttpPost("enableRepositoryExceptionAssault")]
  public void EnableRepositoryExceptionAssault()
  {
    _logger.LogInformation($"Setting {nameof(Assaults.EnableExceptionAssault)}");
    Assaults.Config.EnableExceptionAssault = true;
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