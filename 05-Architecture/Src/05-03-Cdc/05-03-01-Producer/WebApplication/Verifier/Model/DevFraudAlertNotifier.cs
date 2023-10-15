using System.Collections.Concurrent;
using Core.Verifier;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Verifier.Model;

/// <summary>
/// Wersja deweloperska wysyłacza wiadomości.
/// Zamiast brokera, mamy kolejkę w pamięci.
///
/// Ponadto klasa jest kontrolerem WebApi, który pozwala
/// odpytać się o umieszczone w kolejce elementy.
/// </summary>
[ApiController]
[Route("fraudalert")]
public class DevFraudAlertNotifier : IFraudAlertNotifier
{
  private static readonly BlockingCollection<CustomerVerification> Broker
    = new BlockingCollection<CustomerVerification>(50);

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