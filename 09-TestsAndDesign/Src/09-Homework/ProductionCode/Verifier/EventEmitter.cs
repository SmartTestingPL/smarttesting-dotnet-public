namespace ProductionCode.Verifier;

public interface IEventEmitter
{
  void Emit(VerificationEvent @event);
}

public class EventEmitter : IEventEmitter
{
  public void Emit(VerificationEvent @event)
  {
  }
}