namespace BikService.Income;

public class DevIncomeRestTemplateClient : IIncomeRestTemplateClient
{
  public async Task<T> Get<T>(string url)
  {
    return await Task.FromResult((T) (object)"2000");
  }
}