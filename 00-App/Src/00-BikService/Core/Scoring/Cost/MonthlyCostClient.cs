using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Cost;

public interface IMonthlyCostClient
{
  Task<decimal?> GetMonthlyCosts(Pesel pesel);
}

public class MonthlyCostClient : IMonthlyCostClient
{
  private readonly ILogger<MonthlyCostClient> _log;
  private readonly IRestClient _restClient;
  private readonly string _monthlyCostServiceUrl;

  public MonthlyCostClient(IRestClient restClient, string monthlyCostServiceUrl, ILogger<MonthlyCostClient> log)
  {
    _restClient = restClient;
    _monthlyCostServiceUrl = monthlyCostServiceUrl;
    _log = log;
  }

  public async Task<decimal?> GetMonthlyCosts(Pesel pesel)
  {
    var monthlyCostString = await _restClient.Get<string>($"{_monthlyCostServiceUrl}/{pesel.Value}");
    _log.LogInformation($"Monthly cost for id [{pesel}] is [{monthlyCostString}]");
    return decimal.Parse(monthlyCostString);
  }
}
