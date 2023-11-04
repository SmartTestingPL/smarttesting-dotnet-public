using System.Threading;
using System.Threading.Tasks;
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
  /// <param name="cancellationToken"></param>
  /// <returns>rezultat weryfikacji</returns>
  Task<VerificationResult> Passes(
    Person person, CancellationToken cancellationToken);

  /// <summary>
  /// nazwa weryfikacji
  /// </summary>
  string Name => "name";
}