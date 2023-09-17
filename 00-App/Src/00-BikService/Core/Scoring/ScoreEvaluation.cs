using Core.Scoring.domain;

namespace Core.Scoring;

public interface IScoreEvaluation
{
  Task<Score> Evaluate(Pesel pesel);
}
