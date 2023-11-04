using System;
using Core.Verifier.Application;
using Core.Verifier.Model.Verification;

namespace WebApplication;

/// <summary>
/// Fałszywka udająca emiter zdarzeń, w tym przykładzie
/// potrzebna tylko po to, żeby kontener rozwiązał zależności
/// </summary>
public class FakeEventEmitter : IEventEmitter
{
  public void Emit(VerificationEvent verificationEvent)
  {
    Console.WriteLine(verificationEvent);
  }
}