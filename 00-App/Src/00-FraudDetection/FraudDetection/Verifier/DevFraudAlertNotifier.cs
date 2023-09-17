using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Verifier;

/// <summary>
/// Wersja deweloperska wysyłacza wiadomości. Zamiast brokera, mamy kolejkę
/// w pamięci.
/// Wystawiamy dodatkowo końcówkę HTTP z metodą GET w celu
/// wyciągnięcia zapisanych elementów w kolejce.
/// </summary>
[ApiController]
[Route("fraudalert")]
public class DevFraudAlertNotifier : IFraudAlertNotifier
{
  private static readonly BlockingCollection<CustomerVerification> Broker
    = new BlockingCollection<CustomerVerification>(50);

  [HttpPost]
  public void FraudFound(CustomerVerification customerVerification)
  {
    Broker.Add(customerVerification);
  }

  [HttpGet]
  public CustomerVerification[] Poll()
  {
    return Broker.ToArray();
  }
}