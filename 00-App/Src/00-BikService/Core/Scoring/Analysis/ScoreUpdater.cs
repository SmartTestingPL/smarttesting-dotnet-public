using Core.Scoring.domain;

namespace Core.Scoring.Analysis;

public interface IScoreUpdater
{
  void ScoreCalculated(ScoreCalculatedEvent scoreCalculatedEvent);
}
