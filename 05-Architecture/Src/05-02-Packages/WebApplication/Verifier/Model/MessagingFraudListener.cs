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

  public Task OnFraud(CustomerVerification customerVerification)
  {
    _logger.LogInformation($"Got customer verification [{customerVerification}]");
    return _repository.SaveAsync(
      VerifiedPerson.CreateInstance(
        customerVerification.Result.UserId,
        customerVerification.Person.NationalIdentificationNumber,
        customerVerification.Result.Status));
  }
}