using ProductionCode.Events;

namespace ProductionCode.Verifier;

public interface IEventEmitter
{
  void Emit(VerificationEvent @event);
}

/// <summary>
/// Klasa udająca klasę łączącą się po brokerze wiadomości.
/// </summary>
public class EventEmitter : IEventEmitter
{
  public void Emit(VerificationEvent @event)
  {
  }
}