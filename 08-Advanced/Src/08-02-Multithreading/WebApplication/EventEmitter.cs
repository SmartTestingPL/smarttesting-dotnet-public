using Core.Verifier.Application;
using Core.Verifier.Model;
using Core.Verifier.Model.Verification;

namespace WebApplication;

public class EventEmitter : IEventEmitter
{
  private readonly _03_VerificationListener _listener;

  public EventEmitter(_03_VerificationListener listener)
  {
    _listener = listener;
  }

  public void Emit(VerificationEvent verificationEvent)
  {
    _listener.Listen(verificationEvent);
  }
}