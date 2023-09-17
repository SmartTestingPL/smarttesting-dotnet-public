using BikService.Chaos;
using Core.Scoring.domain;
using Core.Scoring.Personal;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace BikService.Personal;

public class OccupationRepositoryWatcher : IOccupationRepository
{
  private readonly EfCoreOccupationRepository _repository;
  private readonly InjectOutcomePolicy _chaosPolicy = MonkeyPolicy.InjectException(with =>
    with.Fault(new Exception("thrown from exception attack!"))
      .InjectionRate(1)
      .EnabledWhen((context, token) => Assaults.Config.EnableExceptionAssault)
  );

  public OccupationRepositoryWatcher(EfCoreOccupationRepository repository)
  {
    _repository = repository;
  }

  public Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    var capturedResult = _chaosPolicy.ExecuteAndCapture(() => _repository.GetOccupationScores());
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