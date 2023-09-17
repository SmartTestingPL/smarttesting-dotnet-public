using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Personal;

/// <summary>
/// Na potrzeby pracy domowej, załóżmy, że ta klasa wykonuje skomplikowane
/// i potencjalnie długotrwałe obliczenia.
/// </summary>
public class PersonalInformationScoreEvaluation : IScoreEvaluation
{
  private readonly ILogger<PersonalInformationScoreEvaluation> _log;
  private readonly IPersonalInformationClient _client;
  private readonly IOccupationRepository _occupationRepository;

  public PersonalInformationScoreEvaluation(IPersonalInformationClient client, IOccupationRepository occupationRepository, ILogger<PersonalInformationScoreEvaluation> log)
  {
    _client = client;
    _occupationRepository = occupationRepository;
    _log = log;
  }

  public async Task<Score> Evaluate(Pesel pesel)
  {
    _log.LogInformation($"Evaluating personal info score for [{pesel}]");
    var personalInformation = await _client.GetPersonalInformation(pesel);
    return Score.Zero
        .Add(ScoreForOccupation(personalInformation.Occupation))
        .Add(ScoreForEducation(personalInformation.Education))
        .Add(ScoreForYearsOfWorkExperience(personalInformation.YearsOfWorkExperience));
  }

  private Score ScoreForOccupation(PersonalInformation.Occupations? occupation)
  {
    var occupationToScore = _occupationRepository.GetOccupationScores();
    _log.LogInformation($"Found following mappings {occupationToScore}");
    var success = occupationToScore.TryGetValue(occupation, out var score);
    _log.LogInformation($"Found score {score} for occupation {occupation}");
    return success ? score : Score.Zero;
  }

  private static Score ScoreForEducation(PersonalInformation.Educations? education)
  {
    return education switch
    {
      PersonalInformation.Educations.Basic => new Score(10),
      PersonalInformation.Educations.High => new Score(50),
      PersonalInformation.Educations.Medium => new Score(30),
      _ => Score.Zero
    };
  }

  private Score ScoreForYearsOfWorkExperience(int yearsOfWorkExperience)
  {
    if (yearsOfWorkExperience == 1)
    {
      return new Score(5);
    }
    else if (yearsOfWorkExperience is >= 2 and < 5)
    {
      return new Score(10);
    }
    else if (yearsOfWorkExperience is >= 5 and < 10)
    {
      return new Score(20);
    }
    else if (yearsOfWorkExperience is >= 10 and < 15)
    {
      return new Score(30);
    }
    else if (yearsOfWorkExperience is >= 15 and < 20)
    {
      return new Score(40);
    }
    else if (yearsOfWorkExperience is >= 20 and < 30)
    {
      return new Score(50);
    }
    return Score.Zero;
  }

}
