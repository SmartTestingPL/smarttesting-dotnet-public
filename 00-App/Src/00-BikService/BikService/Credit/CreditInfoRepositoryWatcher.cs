using BikService.Chaos;
using Core.Scoring.Credit;
using Core.Scoring.domain;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace BikService.Credit;

public class CreditInfoRepositoryWatcher : ICreditInfoRepository, ICreditInfoRepositoryForInitialization
{
  private readonly MongoTemplateCreditInfoRepository _repository;

  private readonly AsyncInjectOutcomePolicy _chaosPolicy = MonkeyPolicy.InjectExceptionAsync(with =>
    with.Fault(new Exception("thrown from exception attack!"))
      .InjectionRate(1)
      .EnabledWhen(async (context, token) => await Task.FromResult(Assaults.Config.EnableExceptionAssault))
  );

  public CreditInfoRepositoryWatcher(MongoTemplateCreditInfoRepository repository)
  {
    _repository = repository;
  }

  public async Task<CreditInfo?> FindCreditInfo(Pesel pesel)
  {
    return RethrowExceptionOccurredDuring(
      await _chaosPolicy.ExecuteAndCaptureAsync(() => _repository.FindCreditInfo(pesel)));
  }

  public async Task<CreditInfo> SaveCreditInfo(Pesel pesel, CreditInfo creditInfo)
  {
    return RethrowExceptionOccurredDuring(
      await _chaosPolicy.ExecuteAndCaptureAsync(() => _repository.SaveCreditInfo(pesel, creditInfo)));
  }

  public async Task Save(CreditInfoDocument creditInfoDocument)
  {
    await _repository.Save(creditInfoDocument);
  }

  public async Task Clear()
  {
    await _repository.Clear();
  }

  private static T RethrowExceptionOccurredDuring<T>(PolicyResult<T> capturedResult)
  {
    if (capturedResult.Outcome == OutcomeType.Failure)
    {
      throw capturedResult.FinalException;
    }
    else
    {
      return capturedResult.Result;
    }
  }
}