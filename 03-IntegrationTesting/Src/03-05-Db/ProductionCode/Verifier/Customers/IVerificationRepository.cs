using System;
using System.Threading.Tasks;
using Core.Maybe;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Interfejs dostępu do bazy danych
/// </summary>
public interface IVerificationRepository
{
  Maybe<VerifiedPerson> FindByUserId(Guid userId);
  Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson);
}