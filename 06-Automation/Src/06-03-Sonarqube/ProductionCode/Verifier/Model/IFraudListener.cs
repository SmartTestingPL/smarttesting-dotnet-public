using System.Threading.Tasks;

namespace ProductionCode.Verifier.Model;

/// <summary>
/// Komponent odpowiedzialny za nasłuchiwanie na wiadomości z oszustem.
/// </summary>
public interface IFraudListener
{
  /// <summary>
  /// Metoda wywołana w momencie, w którym dostaliśmy notyfikację o oszuście.
  /// </summary>
  /// <param name="customerVerification">weryfikacja klienta</param>
  Task OnFraud(CustomerVerification customerVerification);
}