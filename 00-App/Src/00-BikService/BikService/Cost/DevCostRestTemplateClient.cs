namespace BikService.Cost;

public class DevCostRestTemplateClient : ICostRestTemplateClient
{
  public async Task<T> Get<T>(string url)
  {
    return await Task.FromResult((T) (object)"1000");
  }
}
