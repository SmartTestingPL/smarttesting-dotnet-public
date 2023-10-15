using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Maybe;

namespace Core.Verifier;

/// <summary>
/// Kontrakt na zapis i odczyt elementów związanych z weryfikacją.
/// </summary>
public interface IVerificationRepository
{
  /// <summary>
  /// Zapisuje dany obiekt.
  /// </summary>
  /// <param name="verifiedPerson">obiekt do zapisu</param>
  /// <param name="cancellationToken"></param>
  /// <returns>zapisany obiekt</returns>
  Task<VerifiedPerson> SaveAsync(
    VerifiedPerson verifiedPerson,
    CancellationToken cancellationToken);

  /// <summary>
  /// Wyszukuje zweryfikowanej osoby po ID.
  /// </summary>
  /// <param name="userId">id po którym będziemy wyszukiwać osoby</param>
  /// <returns>zweryfikowana osoba opakowana w <see cref="Maybe{T}"/></returns>
  Maybe<VerifiedPerson> FindByUserId(Guid userId);

  /// <returns>suma zapisanych elementów</returns>
  Task<int> Count();

  /// <summary>
  /// Tworzy bazę danych, jeśli ta jeszcze nie istnieje
  /// </summary>
  void EnsureExists();
}