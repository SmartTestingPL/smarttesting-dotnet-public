using Core.Scoring.Cost;
using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Income;

public interface IMonthlyIncomeClient
{
  Task<decimal?> GetMonthlyIncome(Pesel pesel);
}

public class MonthlyIncomeClient : IMonthlyIncomeClient
{
  private readonly ILogger<MonthlyIncomeClient> _log;
  private readonly IRestClient _restClient;
  private readonly string _monthlyIncomeServiceUrl;

  public MonthlyIncomeClient(IRestClient restClient, string monthlyIncomeServiceUrl, ILogger<MonthlyIncomeClient> log)
  {
    _restClient = restClient;
    _monthlyIncomeServiceUrl = monthlyIncomeServiceUrl;
    _log = log;
  }

  public async Task<decimal?> GetMonthlyIncome(Pesel pesel)
  {
    var monthlyIncomeString = await _restClient.Get<string>($"{_monthlyIncomeServiceUrl}/{pesel.Value}");
    var monthlyIncome = decimal.Parse(monthlyIncomeString);
    _log.LogInformation($"Monthly income for id [{pesel}] is [{monthlyIncome}]");
    return monthlyIncome;
  }

}
