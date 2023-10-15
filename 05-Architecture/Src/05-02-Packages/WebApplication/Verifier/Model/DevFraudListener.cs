﻿using System.Threading.Tasks;
using Core.Verifier;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Verifier.Model;

/// <summary>
/// Deweloperska wersja nasłuchiwacza na wiadomości. Dzięki atrybutowi
/// [Controller] wystawiamy endpoint RESTowy, dzięki któremu
/// za pomocą metody POST jesteśmy w stanie zasymulować uzyskanie
/// wiadomości z kolejki.
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
  public Task OnFraud(CustomerVerification customerVerification)
  {
    return _fraudListener.OnFraud(customerVerification);
  }
}