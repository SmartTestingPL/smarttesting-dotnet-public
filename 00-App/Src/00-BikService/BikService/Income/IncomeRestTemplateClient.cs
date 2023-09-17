using BikService.Cost;
using Core.Scoring.Cost;
using Polly;

namespace BikService.Income;

public interface IIncomeRestTemplateClient : IRestClient
{
}

public class IncomeRestTemplateClient : IIncomeRestTemplateClient
{
  private readonly RestTemplate _restTemplate;

  public IncomeRestTemplateClient(RestTemplate restTemplate)
  {
    _restTemplate = restTemplate;
  }

  public async Task<T> Get<T>(string url)
  {
    return await Policy
      .Handle<Exception>()
      .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1))
      .ExecuteAsync(() => _restTemplate.GetForObject<T>(url));
  }
}
