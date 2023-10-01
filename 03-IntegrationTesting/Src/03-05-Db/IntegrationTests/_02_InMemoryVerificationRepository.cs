using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Maybe;
using ProductionCode.Verifier.Customers;

namespace IntegrationTests;

/// <summary>
/// Abstrakcja nad zwykłą mapę symulującą bazę danych. 
/// </summary>
public class _02_InMemoryVerificationRepository : IVerificationRepository
{
  private readonly Dictionary<Guid, VerifiedPerson> _peopleByGuid = new Dictionary<Guid, VerifiedPerson>();

  private IEnumerable<VerifiedPerson> FindAll()
  {
    return _peopleByGuid.Values;
  }

  public Maybe<VerifiedPerson> FindByUserId(Guid number)
  {
    return FindAll().Where(p => p.UserId == number).SingleMaybe();
  }

  public Task<VerifiedPerson> SaveAsync(VerifiedPerson entity)
  {
    _peopleByGuid[entity.UserId] = entity;
    return Task.FromResult(entity);
  }
}