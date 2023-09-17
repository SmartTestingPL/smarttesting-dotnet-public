using Core.Scoring.domain;

namespace Core.Scoring.Analysis;

public interface ICompositeScoreEvaluation
{
  Task<Score> AggregateAllScores(Pesel pesel);
}
