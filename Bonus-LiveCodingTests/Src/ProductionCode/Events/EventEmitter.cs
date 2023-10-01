namespace ProductionCode.Events;

public interface IEventEmitter
{
  void Emit(Event @event);
}

/// <summary>
/// Klasa udająca klasę łączącą się po brokerze wiadomości.
/// </summary>
public class EventEmitter : IEventEmitter
{
  public void Emit(Event @event)
  {
  }
}