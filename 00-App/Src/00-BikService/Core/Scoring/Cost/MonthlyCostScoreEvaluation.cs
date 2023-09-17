using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Cost;

public class MonthlyCostScoreEvaluation : IScoreEvaluation
{
  private readonly ILogger<MonthlyCostScoreEvaluation> _log;

  private readonly IMonthlyCostClient _client;

  public MonthlyCostScoreEvaluation(IMonthlyCostClient client, ILogger<MonthlyCostScoreEvaluation> log)
  {
    _client = client;
    _log = log;
  }

  public async Task<Score> Evaluate(Pesel pesel)
  {
    _log.LogInformation($"Evaluating monthly cost score for [{pesel}]");
    var monthlyCosts = (await _client.GetMonthlyCosts(pesel)).Value;
    if (Between(monthlyCosts, "0", "500"))
    {
      return new Score(50);
    }
    else if (Between(monthlyCosts, "501", "1500"))
    {
      return new Score(40);
    }
    else if (Between(monthlyCosts, "1501", "3500"))
    {
      return new Score(30);
    }
    else if (Between(monthlyCosts, "5501", "10000"))
    {
      return new Score(10);
    }
    return Score.Zero;
  }

  private bool Between(decimal income, string min, string max)
  {
    return income.CompareTo(decimal.Parse(min)) >= 0 && income.CompareTo(decimal.Parse(max)) < 0;
  }
}
