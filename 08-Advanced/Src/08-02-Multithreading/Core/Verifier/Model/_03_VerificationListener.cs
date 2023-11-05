using System.Collections.Concurrent;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model;

/// <summary>
/// Nasłuchiwacz na zdarzenia weryfikacyjne.
/// Zapisuje je w kolejce.
/// </summary>
public class _03_VerificationListener
{
  private readonly ILogger<_03_VerificationListener> _logger;

  public _03_VerificationListener(ILogger<_03_VerificationListener> logger)
  {
    _logger = logger;
  }


  public BlockingCollection<VerificationEvent> Events
    = new BlockingCollection<VerificationEvent>();

  /// <summary> 
  /// Metoda uruchomi się w momencie uzyskania zdarzenia
  /// typu <see cref="VerificationEvent"/>.
  /// </summary>
  /// <param name="event">zdarzenie do obsłużenia</param>
  public void Listen(VerificationEvent @event)
  {
    _logger.LogInformation($"Got an event! [{@event}]");
    Events.Add(@event);
  }
}