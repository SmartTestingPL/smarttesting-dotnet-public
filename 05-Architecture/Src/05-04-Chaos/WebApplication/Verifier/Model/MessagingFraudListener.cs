using System.Threading;
using System.Threading.Tasks;
using Core.Verifier;
using Microsoft.Extensions.Logging;

namespace WebApplication.Verifier.Model;

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
  /// Metoda wykonywana, gdy w kolejce pojawi się nowy obiekt
  /// </summary>
  /// <param name="customerVerification">weryfikacja klienta</param>
  /// <param name="cancellationToken"></param>
  public Task OnFraud(CustomerVerification customerVerification, CancellationToken cancellationToken)
  {
    _logger.LogInformation($"Got customer verification [{customerVerification}]");
    return _repository.SaveAsync(
      VerifiedPerson.CreateInstance(
        customerVerification.Result.UserId,
        customerVerification.Person.NationalIdentificationNumber,
        customerVerification.Result.Status),
      cancellationToken);
  }
}