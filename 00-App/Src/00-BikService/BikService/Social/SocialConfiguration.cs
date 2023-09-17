using Core.Scoring.Social;

namespace BikService.Social;

public class DevSocialRestTemplateClient : ISocialRestTemplateClient
{
  public async Task<T> Get<T>(string url)
  {
    return await Task.FromResult((T) (object) new SocialStatus(1, 2, 
      SocialStatus.MaritalStatuses.Married, 
      SocialStatus.ContractTypes.EmploymentContract));
  }
}
