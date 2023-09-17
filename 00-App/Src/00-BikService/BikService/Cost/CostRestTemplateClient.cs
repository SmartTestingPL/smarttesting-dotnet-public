using Core.Scoring.Cost;
using Polly;

namespace BikService.Cost;

public interface ICostRestTemplateClient : IRestClient
{
}

public class CostRestTemplateClient : ICostRestTemplateClient
{
  private readonly RestTemplate _restTemplate;

  public CostRestTemplateClient(RestTemplate restTemplate)
  {
    _restTemplate = restTemplate;
  }

  public async Task<T> Get<T>(string url)
  {
    return await Policy
      .Handle<Exception>()
      .CircuitBreakerAsync(100, TimeSpan.FromMinutes(1))
      .ExecuteAsync(async () => await _restTemplate.GetForObject<T>(url));
  }
}