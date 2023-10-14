using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Verifier;
using Core.Maybe;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace WebApplication.Verifier.Infrastructure;

/// <summary>
/// Dekorator na obiekt dostępu do bazy danych.
/// Używa biblioteki Simmy do wstrzykiwania wyjątków przy dostępie
/// do bazy danych.
/// </summary>
public class VerificationRepositoryWatcher : IVerificationRepository
{
  /// <summary>
  /// Opakowane repozytorium
  /// </summary>
  private readonly IVerificationRepository _repository;

  /// <summary>
  /// Polityka wstrzykiwania wyjątków
  /// </summary>
  private readonly InjectOutcomePolicy _chaosPolicy = MonkeyPolicy.InjectException(with =>
    with.Fault(new Exception("thrown from exception attack!"))
      .InjectionRate(1) // wstrzykujemy w 100% przypadków
      .EnabledWhen((context, token) => Assaults.Config.EnableExceptionAssault)
  );

  public VerificationRepositoryWatcher(IVerificationRepository repository)
  {
    _repository = repository;
  }

  public Maybe<VerifiedPerson> FindByUserId(Guid userId)
  {
    return _chaosPolicy.Execute(() => _repository.FindByUserId(userId));
  }

  public Task<VerifiedPerson> SaveAsync(
    VerifiedPerson verifiedPerson, 
    CancellationToken cancellationToken)
  {
    return _chaosPolicy.Execute(
      async () => await _repository.SaveAsync(
        verifiedPerson, cancellationToken));
  }

  public Task<int> Count()
  {
    return _chaosPolicy.Execute(() => _repository.Count());
  }

  public void EnsureExists()
  {
    _chaosPolicy.Execute(() => _repository.EnsureExists());
  }
}