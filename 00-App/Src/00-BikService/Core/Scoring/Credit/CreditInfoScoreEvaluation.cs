using Core.Scoring.domain;
using Microsoft.Extensions.Logging;

namespace Core.Scoring.Credit;

public class CreditInfoScoreEvaluation : IScoreEvaluation
{

  private readonly ILogger<CreditInfoScoreEvaluation> _log;

  private readonly ICreditInfoRepository _creditInfoRepository;

  public CreditInfoScoreEvaluation(ICreditInfoRepository creditInfoRepository, ILogger<CreditInfoScoreEvaluation> log)
  {
    _creditInfoRepository = creditInfoRepository;
    _log = log;
  }

  public async Task<Score> Evaluate(Pesel pesel)
  {
    _log.LogInformation($"Evaluating credit info score for {pesel}");
    var creditInfo = await _creditInfoRepository.FindCreditInfo(pesel);
    if (creditInfo == null)
    {
      return Score.Zero;
    }
    return Score.Zero
        .Add(ScoreForCurrentDebt(creditInfo.CurrentDebt))
        .Add(ScoreForCurrentLivingCosts(creditInfo.CurrentLivingCosts))
        .Add(ScoreForDebtPaymentHistory(creditInfo.DebtPaymentHistory));
  }

  private Score ScoreForCurrentDebt(decimal? currentDebt)
  {
    if (Between(currentDebt, "5501", "10000"))
    {
      return Score.Zero;
    }
    else if (Between(currentDebt, "3501", "5500"))
    {
      return new Score(10);
    }
    else if (Between(currentDebt, "1501", "3500"))
    {
      return new Score(20);
    }
    else if (Between(currentDebt, "500", "1500"))
    {
      return new Score(40);
    }
    return new Score(50);
  }

  private Score ScoreForCurrentLivingCosts(decimal? currentDebt)
  {
    if (Between(currentDebt, "6501", "10000"))
    {
      return Score.Zero;
    }
    else if (Between(currentDebt, "4501", "6500"))
    {
      return new Score(10);
    }
    else if (Between(currentDebt, "2501", "4500"))
    {
      return new Score(20);
    }
    else if (Between(currentDebt, "1000", "2500"))
    {
      return new Score(40);
    }
    return new Score(50);
  }

  private Score ScoreForDebtPaymentHistory(CreditInfo.DebtPaymentHistoryStatus? debtPaymentHistory)
  {
    return debtPaymentHistory switch
    {
      CreditInfo.DebtPaymentHistoryStatus.MultipleUnpaidInstallments => new Score(10),
      CreditInfo.DebtPaymentHistoryStatus.NotASingleUnpaidInstallment => new Score(50),
      CreditInfo.DebtPaymentHistoryStatus.IndividualUnpaidInstallments => new Score(30),
      _ => Score.Zero
    };
  }

  private bool Between(decimal? income, string min, string max)
  {
    return income.Value.CompareTo(decimal.Parse(min)) >= 0 && income.Value.CompareTo(decimal.Parse(max)) <= 0;
  }
}
