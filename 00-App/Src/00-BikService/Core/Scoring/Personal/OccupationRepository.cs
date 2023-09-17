using Core.Scoring.domain;

namespace Core.Scoring.Personal;

public interface IOccupationRepository
{
  Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores();
}
