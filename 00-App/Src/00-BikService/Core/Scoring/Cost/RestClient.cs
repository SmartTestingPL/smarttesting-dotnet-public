namespace Core.Scoring.Cost;

public interface IRestClient
{
  public Task<T> Get<T>(string url);
}