using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.Verifier;

/// <summary>
/// Deweloperska wersja nasłuchiwacza na wiadomości. Dzięki atrybutowi
/// <see cref="ApiControllerAttribute"/> wystawiamy endpoint RESTowy,
/// dzięki któremu za pomocą metody POST jesteśmy w stanie zasymulować
/// uzyskanie wiadomości z kolejki.
/// </summary>
[ApiController]
[Route("fraud")]
public class DevFraudListener : IFraudListener
{
  private readonly IFraudListener _fraudListener;

  public DevFraudListener(IFraudListener fraudListener)
  {
    _fraudListener = fraudListener;
  }

  [HttpPost]
  public async Task OnFraud(CustomerVerification customerVerification)
  {
    await _fraudListener.OnFraud(customerVerification);
  }
}