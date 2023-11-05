using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po wieku.
/// Po zakończonym procesowaniu weryfikacji wysyła zdarzenie
/// z rezultatem weryfikacji.
/// </summary>
public class AgeVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;
  private readonly ILogger<AgeVerification> _logger;

  public AgeVerification(IEventEmitter eventEmitter, ILogger<AgeVerification> logger)
  {
    _eventEmitter = eventEmitter;
    _logger = logger;
  }

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na Taskach.
  /// </summary>
  public async Task<VerificationResult> PassesAsync(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running age verification");

    // Symuluje przetwarzanie w losowym czasie do 2 sekund
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);

    if (person.GetAge() < 0)
    {
      _logger.LogWarning("Age is negative");
      throw new InvalidOperationException("Age cannot be negative.");
    }

    _logger.LogInformation("Age verification done");
    var result = person.GetAge() >= 18 && person.GetAge() <= 99;
    _eventEmitter.Emit(new VerificationEvent(this, "age", result));
    return new VerificationResult("age", result);
  }

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na zwykłych wątkach.
  /// </summary>
  public VerificationResult Passes(Person person)
  {
    _logger.LogInformation("Running age verification");
    // Symuluje przetwarzanie w losowym czasie do 2 sekund
    Thread.Sleep(new Random().Next(2000));

    if (person.GetAge() < 0)
    {
      _logger.LogWarning("Age is negative");
      throw new InvalidOperationException("Age cannot be negative.");
    }

    _logger.LogInformation("Age verification done");
    var result = person.GetAge() >= 18 && person.GetAge() <= 99;
    _eventEmitter.Emit(new VerificationEvent(this, "age", result));
    return new VerificationResult("age", result);
  }
}