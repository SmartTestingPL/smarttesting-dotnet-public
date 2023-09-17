using Core.Scoring.domain;

namespace Core.Scoring.Social;

public class SocialStatusScoreEvaluation : IScoreEvaluation
{
  private readonly ISocialStatusClient _client;

  public SocialStatusScoreEvaluation(ISocialStatusClient client)
  {
    _client = client;
  }

  public async Task<Score> Evaluate(Pesel pesel)
  {
    var socialStatus = await _client.GetSocialStatus(pesel);
    return Score.Zero
        .Add(ScoreForNoOfDependants(socialStatus))
        .Add(ScoreForNoOfPeopleInTheHousehold(socialStatus))
        .Add(ScoreForMaritalStatus(socialStatus))
        .Add(ScoreForContractType(socialStatus));
  }

  private static Score ScoreForNoOfDependants(SocialStatus socialStatus)
  {
    if (socialStatus.NoOfDependants == 0)
    {
      return new Score(50);
    }
    if (socialStatus.NoOfDependants == 1)
    {
      return new Score(40);
    }
    else if (socialStatus.NoOfDependants == 2)
    {
      return new Score(30);
    }
    else if (socialStatus.NoOfDependants == 3)
    {
      return new Score(20);
    }
    else if (socialStatus.NoOfDependants == 4)
    {
      return new Score(10);
    }
    return Score.Zero;
  }


  private static Score ScoreForNoOfPeopleInTheHousehold(SocialStatus socialStatus)
  {
    if (socialStatus.NoOfPeopleInTheHousehold == 1)
    {
      return new Score(50);
    }
    else if (socialStatus.NoOfPeopleInTheHousehold > 1 && socialStatus.NoOfPeopleInTheHousehold <= 2)
    {
      return new Score(40);
    }
    else if (socialStatus.NoOfPeopleInTheHousehold > 2 && socialStatus.NoOfPeopleInTheHousehold < 3)
    {
      return new Score(30);
    }
    else if (socialStatus.NoOfPeopleInTheHousehold > 3 && socialStatus.NoOfPeopleInTheHousehold <= 4)
    {
      return new Score(20);
    }
    else if (socialStatus.NoOfPeopleInTheHousehold > 4 && socialStatus.NoOfPeopleInTheHousehold <= 5)
    {
      return new Score(10);
    }
    return Score.Zero;
  }

  private static Score ScoreForMaritalStatus(SocialStatus socialStatus)
  {
    return socialStatus.MaritalStatus switch
    {
      SocialStatus.MaritalStatuses.Single => new Score(20),
      SocialStatus.MaritalStatuses.Married => new Score(10),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  private static Score ScoreForContractType(SocialStatus socialStatus)
  {
    return socialStatus.ContractType switch
    {
      SocialStatus.ContractTypes.EmploymentContract => new Score(20),
      SocialStatus.ContractTypes.OwnBusinessActivity => new Score(10),
      _ => Score.Zero
    };
  }

}
