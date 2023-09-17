using Core.Scoring.domain;

namespace Core.Scoring.Credit;

public interface ICreditInfoRepository
{
  Task<CreditInfo?> FindCreditInfo(Pesel pesel);
  Task<CreditInfo> SaveCreditInfo(Pesel pesel, CreditInfo creditInfo);
}
