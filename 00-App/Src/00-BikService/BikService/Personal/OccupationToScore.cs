using Core.Scoring.Personal;

namespace BikService.Personal;

public class OccupationToScore
{
  public int Id { get; set; }
  public PersonalInformation.Occupations Occupation { get; set; }
  public int OccupationScore { get; set; }
}