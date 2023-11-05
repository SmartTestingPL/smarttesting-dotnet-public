using System.Threading;
using System.Threading.Tasks;
using Core.Customers;

namespace Core.Verifier.Model;

/// <summary>
/// Weryfikacja klienta
/// </summary>
public interface IVerification
{
  /// <returns>nazwa weryfikacji</returns>
  string Name => "name";

  /// <summary> 
  /// Weryfikuje czy dana osoba nie jest oszustem.
  /// Wersja działająca na zadaniach.
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns> 
  Task<VerificationResult> PassesAsync(Person person, CancellationToken cancellationToken);

  /// <summary> 
  /// Weryfikuje czy dana osoba nie jest oszustem.
  /// Wersja działająca na zwykłych wątkach.
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns> 
  VerificationResult Passes(Person person);
}