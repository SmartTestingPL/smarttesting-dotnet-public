using Core.Verifier.Application;

namespace Core.Verifier.Model.Verification;

public interface IEventEmitter
{
  void Emit(VerificationEvent verificationEvent);
}