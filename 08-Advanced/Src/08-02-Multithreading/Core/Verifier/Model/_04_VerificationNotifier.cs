using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model;

/// <summary>
/// Powiadamia o znalezionych oszustach.
/// </summary>
public class _04_VerificationNotifier : IFraudAlertNotifier
{
  private readonly ILogger<_04_VerificationNotifier> _logger;

  public _04_VerificationNotifier(ILogger<_04_VerificationNotifier> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// W metodzie odpalamy nowe zadanie (task),
  /// co domyślnie spowoduje uruchomienie kodu w osobnym
  /// wątku.
  /// </summary>
  public virtual async Task FraudFound(
    CustomerVerification customerVerification)
  {
    await Task.Run(async () =>
    {
      _logger.LogInformation("Running fraud notification in a new task");
      await Task.Delay(TimeSpan.FromMilliseconds(2000));
    });
  }
}