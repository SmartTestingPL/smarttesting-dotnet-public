using Core.Scoring.Personal;

namespace BikService.Personal;

public class DevPersonalRestTemplateClient : IPersonalRestTemplateClient
{
  public async Task<T> Get<T>(string url)
  {
    return await Task.FromResult((T) (object) new PersonalInformation(
      PersonalInformation.Educations.Basic, 
      10, 
      PersonalInformation.Occupations.Doctor));
  }
}