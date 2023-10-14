namespace Core.Verifier;

/// <summary>
/// Komponent odpowiedzialny za wysyłanie wiadomości z oszustem.
/// </summary>
public interface IFraudAlertNotifier
{
  /// <summary>
  /// Metoda wywołana w momencie, w którym znaleziono oszusta.
  /// </summary>
  /// <param name="customerVerification">weryfikacja klienta</param> 
  void FraudFound(CustomerVerification customerVerification);
}