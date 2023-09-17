using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier;

/// <summary>
/// Implementacja nasłuchująca na wiadomości z brokera RabbitMQ.
/// </summary>
public class MessagingFraudListener : IFraudListener
{
  private readonly ILogger<MessagingFraudListener> _logger;
  private readonly IVerificationRepository _repository;

  public MessagingFraudListener(
    IVerificationRepository repository,
    ILogger<MessagingFraudListener> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  /// <summary>
  /// Obsługa obiektu otrzymanego z kolejki
  /// </summary>
  public async Task OnFraud(CustomerVerification customerVerification)
  {
    _logger.LogInformation($"Got customer verification [{customerVerification}]");
    await _repository.SaveAsync(
      VerifiedPerson.CreateInstance(
        customerVerification.Result.UserId,
        customerVerification.Person.NationalIdentificationNumber,
        customerVerification.Result.Status));
  }
}