using Core.Scoring.Cost;
using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Personal;

public interface IPersonalInformationClient
{
  Task<PersonalInformation> GetPersonalInformation(Pesel pesel);
}

public class PersonalInformationClient : IPersonalInformationClient
{
  private readonly ILogger<PersonalInformationClient> _log;
  private readonly IRestClient _restClient;
  private readonly string _personalInformationServiceUrl;

  public PersonalInformationClient(
    IRestClient restClient, 
    string monthlyIncomeServiceUrl, 
    ILogger<PersonalInformationClient> log)
  {
    _restClient = restClient;
    _personalInformationServiceUrl = monthlyIncomeServiceUrl;
    _log = log;
  }

  public async Task<PersonalInformation> GetPersonalInformation(Pesel pesel)
  {
    var personalInformation = await _restClient.Get<PersonalInformation>($"{_personalInformationServiceUrl}/{pesel.Value}");
    _log.LogInformation($"Personal information for id [{pesel}] is [{personalInformation}]");
    return personalInformation;
  }

}
