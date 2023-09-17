using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Income;

public class MonthlyIncomeScoreEvaluation : IScoreEvaluation
{

  private readonly ILogger<MonthlyIncomeScoreEvaluation> _log;

  private readonly IMonthlyIncomeClient _client;

  public MonthlyIncomeScoreEvaluation(
    IMonthlyIncomeClient client, 
    ILogger<MonthlyIncomeScoreEvaluation> log)
  {
    _client = client;
    _log = log;
  }

  public async Task<Score> Evaluate(Pesel pesel)
  {
    _log.LogInformation($"Evaluating monthly income score for [{pesel}]");
    var monthlyIncome = (await _client.GetMonthlyIncome(pesel)).Value;
    if (Between(monthlyIncome, "0", "500"))
    {
      return Score.Zero;
    }
    else if (Between(monthlyIncome, "501", "1500"))
    {
      return new Score(10);
    }
    else if (Between(monthlyIncome, "1501", "3500"))
    {
      return new Score(20);
    }
    else if (Between(monthlyIncome, "5501", "10000"))
    {
      return new Score(40);
    }
    return new Score(50);
  }

  private bool Between(decimal income, string min, string max)
  {
    return income.CompareTo(decimal.Parse(min)) >= 0 && income.CompareTo(decimal.Parse(max)) <= 0;
  }
}
