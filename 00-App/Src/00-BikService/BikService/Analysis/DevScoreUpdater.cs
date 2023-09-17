using Core.Scoring.Analysis;
using Core.Scoring.domain;

namespace BikService.Analysis;

public class DevScoreUpdater : IScoreUpdater
{
  private readonly ILogger<DevScoreUpdater> _log;

  public DevScoreUpdater(ILogger<DevScoreUpdater> log)
  {
    _log = log;
  }

  public void ScoreCalculated(ScoreCalculatedEvent scoreCalculatedEvent)
  {
    _log.LogInformation($"Got the event {scoreCalculatedEvent}");
  }
}

