using Flurl.Http;

namespace BikService.Cost;

public class RestTemplate
{
  private readonly FlurlClient _client;

  public RestTemplate(HttpClient httpClient)
  {
    _client = new FlurlClient(httpClient)
    {
      Settings =
      {
        Timeout = TimeSpan.FromSeconds(3)
      }
    };
  }

  public async Task<T> GetForObject<T>(string url)
  {
    if (typeof(T) == typeof(string))
    {
      return (T)(object) await _client.Request(url).GetStringAsync();
    }
    else
    {
      return await _client.Request(url).GetJsonAsync<T>();
    }
  }
}