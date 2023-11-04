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

  public async Task<VerificationResult> Passes(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running name verification");
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);
    _logger.LogInformation("Name verification done");

    var result = person.Name.All(char.IsLetter);
    _eventEmitter.Emit(new VerificationEvent(this, result));

    return new VerificationResult("name", result);
  }
}