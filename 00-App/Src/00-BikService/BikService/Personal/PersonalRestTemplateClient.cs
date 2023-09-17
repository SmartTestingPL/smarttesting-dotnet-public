using BikService.Cost;
using Core.Scoring.Cost;
using Polly;

namespace BikService.Personal;

public interface IPersonalRestTemplateClient : IRestClient
{
}

public class PersonalRestTemplateClient : IPersonalRestTemplateClient
{
  private readonly RestTemplate _restTemplate;

  public PersonalRestTemplateClient(RestTemplate restTemplate)
  {
    _restTemplate = restTemplate;
  }

  public Task<T> Get<T>(string url)
  {
    return Policy
      .Handle<Exception>()
      .CircuitBreaker(2, TimeSpan.FromMinutes(1))
      .Execute(() => _restTemplate.GetForObject<T>(url));
  }

}
