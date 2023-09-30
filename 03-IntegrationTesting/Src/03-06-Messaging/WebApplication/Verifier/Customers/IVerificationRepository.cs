using System;
using System.Threading.Tasks;
using Core.Maybe;

namespace WebApplication.Verifier.Customers;

/// <summary>
/// Interfejs dostępu do bazy danych
/// </summary>
public interface IVerificationRepository
{
  /// <summary>
  /// Wyszukiwanie osoby po ID
  /// </summary>
  /// <param name="number">numer po którym szukamy użytkownika</param>
  /// <returns>Znaleziona osoba albo jej brak (Maybe.Nothing)</returns>
  Maybe<VerifiedPerson> FindByUserId(Guid number);

  /// <summary>
  /// Zapisuje zweryfikowaną osobę w bazie danych
  /// </summary>
  Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson);
}