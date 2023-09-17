using ProductionCode.Customers;

namespace ProductionCode.Verifier;

/// <summary>
/// Weryfikacja klienta
/// </summary>
public interface IVerification
{
  /// <summary>
  /// Weryfikuje czy dana osoba nie jest oszustem
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>false dla oszusta</returns>
  bool Passes(Person person);
}