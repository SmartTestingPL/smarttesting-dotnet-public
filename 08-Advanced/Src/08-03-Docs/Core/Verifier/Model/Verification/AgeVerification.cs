using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po wieku. Osoba w odpowiednim wieku zostanie
/// zweryfikowana pozytywnie.
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

  public async Task<VerificationResult> Passes(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running age verification");
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);
      
    if (person.GetAge() <= 0)
    {
      _logger.LogWarning("Age is negative");
      throw new InvalidOperationException("Age cannot be negative.");
    }

    _logger.LogInformation("Age verification done");
    var result = person.GetAge() >= 18 && person.GetAge() <= 99;
    _eventEmitter.Emit(new VerificationEvent(this, result));
    return new VerificationResult("age", result);
  }
}