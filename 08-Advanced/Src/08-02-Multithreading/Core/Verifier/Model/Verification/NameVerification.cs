using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po nazwisku.
/// Po zakończonym procesowaniu weryfikacji wysyła
/// zdarzenie z rezultatem weryfikacji.
/// </summary>
public class NameVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;
  private readonly ILogger<NameVerification> _logger;

  public NameVerification(IEventEmitter eventEmitter, ILogger<NameVerification> logger)
  {
    _eventEmitter = eventEmitter;
    _logger = logger;
  }

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na zadaniach (taskach).
  /// </summary>
  public async Task<VerificationResult> PassesAsync(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running name verification");
    // Symuluje procesowanie w losowym czasie do 2 sekund
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);
    _logger.LogInformation("Name verification done");

    var result = person.Name.All(char.IsLetter);
    _eventEmitter.Emit(new VerificationEvent(this, "name", result));

    return new VerificationResult("name", result);
  }

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na zwykłych wątkach.
  /// </summary>
  public VerificationResult Passes(Person person)
  {
    _logger.LogInformation("Running name verification");
    // Symuluje procesowanie w losowym czasie do 2 sekund
    Thread.Sleep(new Random().Next(2000));
    _logger.LogInformation("Name verification done");

    var result = !string.IsNullOrWhiteSpace(person.Name);
    _eventEmitter.Emit(new VerificationEvent(this, "name", result));

    return new VerificationResult("name", result);
  }
}