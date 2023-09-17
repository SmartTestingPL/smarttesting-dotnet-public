using System.Net.Mime;
using Core.Scoring.Cost;
using Flurl.Http;
using Polly;

namespace BikService.Social;

public interface ISocialRestTemplateClient : IRestClient
{
}

public class SocialRestTemplateClient : ISocialRestTemplateClient
{
  private readonly FlurlClient _flurlClient;

  public SocialRestTemplateClient(HttpClient httpClient)
  {
    _flurlClient = new FlurlClient(httpClient)
    {
      Settings =
      {
        Timeout = TimeSpan.FromSeconds(3)
      }
    };
  }

  public async Task<T> Get<T>(string url)
  {
    return await Policy
      .Handle<Exception>()
      .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1))
      .ExecuteAsync(async () => await _flurlClient.Request(url)
        .WithHeader("Content-Type", MediaTypeNames.Application.Json).GetJsonAsync<T>());
  }
}