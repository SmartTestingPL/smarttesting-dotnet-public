using Core.Scoring.domain;
using Core.Scoring.Personal;

namespace BikService.Personal;

public class DevOccupationRepository : IOccupationRepository
{
  public Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    return new Dictionary<PersonalInformation.Occupations?, Score>
    {
      [PersonalInformation.Occupations.Doctor] = new(100)
    };
  }
}
