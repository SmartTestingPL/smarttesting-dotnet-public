using System.Threading.Tasks;

namespace Core.Verifier.Model;

/// <summary>
/// Komponent odpowiedzialny za wysyłanie wiadomości z oszustem.
/// </summary>
public interface IFraudAlertNotifier
{
  /// <summary>
  /// Metoda wywołana w momencie, w którym znaleziono oszusta.
  /// </summary>
  /// <param name="customerVerification">weryfikacja klienta</param>
  Task FraudFound(CustomerVerification customerVerification);
}