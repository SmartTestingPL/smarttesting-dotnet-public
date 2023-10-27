namespace FraudVerifier.Customers.Verification;

/// <summary>
/// Jakiś nowy typ weryfikacji, który chcemy dodać przy kolejnym wydaniu.
/// Zostanie on wprowadzony do kodu na produkcję przy wdrożeniu,
/// ale uruchamiać go będziemy dopiero za pomocą przełącznika.
/// </summary>
public class NewTypeOfVerification : IVerification
{
  public bool Passes(Person person) => false;
}