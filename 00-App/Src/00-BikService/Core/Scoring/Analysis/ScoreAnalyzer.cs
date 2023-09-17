using Core.Scoring.domain;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Core.Scoring.Analysis;

public interface IScoreAnalyzer
{
  Task<bool> ShouldGrantLoan(Pesel pesel);
}

public class ScoreAnalyzer : IScoreAnalyzer
{
  private readonly ILogger<ScoreAnalyzer> _log;
  private readonly ICompositeScoreEvaluation _compositeScoreEvaluation;
  private readonly int _threshold;

  private static readonly Histogram DistributionSummary = Metrics.CreateHistogram("score_aggregated", "Aggregated Score");

  public ScoreAnalyzer(
    ICompositeScoreEvaluation compositeScoreEvaluation,
    int threshold,
    ILogger<ScoreAnalyzer> log)
  {
    _compositeScoreEvaluation = compositeScoreEvaluation;
    _threshold = threshold;
    _log = log;
  }

  public async Task<bool> ShouldGrantLoan(Pesel pesel)
  {
    var aggregateScore = await _compositeScoreEvaluation.AggregateAllScores(pesel);
    var points = aggregateScore.Points;
    DistributionSummary.Observe(points);
    var aboveThreshold = points >= _threshold;
    _log.LogInformation(
      $"For PESEL [{pesel}] we got score [{points}]. It's [{aboveThreshold}] that it's above or equal to the threshold [{_threshold}]");
    return aboveThreshold;
  }
}