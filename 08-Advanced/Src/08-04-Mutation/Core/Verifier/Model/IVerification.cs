using Core.Customers;

namespace Core.Verifier.Model;

/// <summary>
/// Weryfikacja klienta.
/// </summary>
public interface IVerification
{
  /// <summary>
  /// Weryfikuje czy dana osoba nie jest oszustem.
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  VerificationResult Passes(Person person);

  /// <summary>
  /// nazwa weryfikacji
  /// </summary>
  string Name => "name";
}