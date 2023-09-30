namespace ProductionCode.Client;

/// <summary>
/// Weryfikacja klienta.
/// </summary>
public interface IVerification
{
  /// <summary>
  /// Weryfikuje czy dana osoba nie jest oszustem.
  /// </summary>
  /// <returns>false dla oszusta</returns>
  bool Passes() => false;
}