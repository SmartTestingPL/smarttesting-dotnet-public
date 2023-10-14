using System;
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
  /// <param name="entity">obiekt do zapisu</param>
  /// <returns>zapisany obiekt</returns>
  Task<VerifiedPerson> SaveAsync(VerifiedPerson entity);

  /// <summary>
  /// Wyszukuje zweryfikowanej osoby po ID.
  /// </summary>
  /// <param name="number">id po którym będziemy wyszukiwać osoby</param>
  /// <returns>zweryfikowana osoba opakowana w <see cref="Maybe{T}"/></returns>
  Maybe<VerifiedPerson> FindByUserId(Guid number);

  /// <returns>suma zapisanych elementów</returns>
  Task<int> Count();

  /// <summary>
  /// Jeśli trzeba, tworzy bazę danych i wykonuje migrację
  /// </summary>
  void EnsureCreated();
}