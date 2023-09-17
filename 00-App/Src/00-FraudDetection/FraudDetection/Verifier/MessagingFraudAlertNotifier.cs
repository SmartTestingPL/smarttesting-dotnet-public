using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier;

/// <summary>
/// Implementacja notyfikowania o oszuście. Wysyła wiadomość do brokera RabbitMQ.
/// </summary>
public class MessagingFraudAlertNotifier : IFraudAlertNotifier
{
  private readonly ILogger<MessagingFraudAlertNotifier> _logger;

  private readonly IFraudDestination _fraudDestination;

  public MessagingFraudAlertNotifier(
    IFraudDestination fraudDestination,
    ILogger<MessagingFraudAlertNotifier> logger)
  {
    _logger = logger;
    _fraudDestination = fraudDestination;
  }

  public void FraudFound(CustomerVerification customerVerification)
  {
    _logger.LogInformation("Emitting message");
    _fraudDestination.Send("fraudOutput", customerVerification);
  }
}