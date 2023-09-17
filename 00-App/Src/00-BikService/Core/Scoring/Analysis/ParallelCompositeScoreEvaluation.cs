using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Analysis;

public class ParallelCompositeScoreEvaluation : ICompositeScoreEvaluation
{
  private readonly List<IScoreEvaluation> _scoreEvaluations;
  private readonly IScoreUpdater _scoreUpdater;
  private readonly ILogger<ParallelCompositeScoreEvaluation> _logger;
  private readonly IExecutor _executor;

  public ParallelCompositeScoreEvaluation(
    List<IScoreEvaluation> scoreEvaluations,
    IScoreUpdater scoreUpdater,
    IExecutor executor,
    ILogger<ParallelCompositeScoreEvaluation> logger)
  {
    _scoreEvaluations = scoreEvaluations;
    _scoreUpdater = scoreUpdater;
    _logger = logger;
    _executor = executor;
  }

  public async Task<Score> AggregateAllScores(Pesel pesel)
  {
    var score = (await Task.WhenAll(_scoreEvaluations.Select(
        se => _executor.Run(() => se.Evaluate(pesel)))
      .Select(async task => await GetScore(task))))
      .Aggregate(Score.Zero, (a, b) => a.Add(b));
    _logger.LogInformation($"Calculated score {score} for pesel {pesel}");
    _scoreUpdater.ScoreCalculated(new ScoreCalculatedEvent(pesel, score));
    return score;
  }

  private async Task<Score> GetScore(Task<Score> sf)
  {
    try
    {
      return await sf.WaitAsync(TimeSpan.FromMinutes(1));
    }
    catch (Exception e)
    {
      throw new InvalidOperationException("Failed while getting score", e);
    }
  }
}
