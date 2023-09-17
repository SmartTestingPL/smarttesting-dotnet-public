using Core.Scoring.Cost;
using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Social;

public interface ISocialStatusClient
{
  Task<SocialStatus> GetSocialStatus(Pesel pesel);
}

public class SocialStatusClient : ISocialStatusClient
{
  private readonly ILogger<SocialStatusClient> _log;
  private readonly IRestClient _restClient;
  private readonly string _socialStatusServiceUrl;

  public SocialStatusClient(IRestClient restClient, string monthlyIncomeServiceUrl, ILogger<SocialStatusClient> log)
  {
    _restClient = restClient;
    _socialStatusServiceUrl = monthlyIncomeServiceUrl;
    _log = log;
  }

  public async Task<SocialStatus> GetSocialStatus(Pesel pesel)
  {
    var socialStatus = await _restClient.Get<SocialStatus>($"{_socialStatusServiceUrl}/{pesel.Value}");
    _log.LogInformation($"Social status for id [{pesel}] is [{socialStatus}]");
    return socialStatus;
  }

}
