using System.Collections.Concurrent;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model;

/// <summary>
/// Nasłuchiwacz na zdarzenia weryfikacyjne. Zapisuje je w kolejce.
/// </summary>
public class VerificationListener
{
  private readonly ILogger<VerificationListener> _logger;

  public VerificationListener(ILogger<VerificationListener> logger)
  {
    _logger = logger;
  }

  public readonly BlockingCollection<VerificationEvent> Events
    = new BlockingCollection<VerificationEvent>();

  /// <summary>
  /// Metoda uruchomi się w momencie uzyskania zdarzenia typu
  /// <see cref="VerificationEvent"/>
  /// </summary>
  /// <param name="event">zdarzenie do obsłużenia</param>
  public void Listen(VerificationEvent @event)
  {
    _logger.LogInformation($"Got an event! [{@event}]");
    Events.Add(@event);
  }
}