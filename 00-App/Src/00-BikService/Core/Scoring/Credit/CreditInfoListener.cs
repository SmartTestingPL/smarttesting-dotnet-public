using Core.Scoring.domain;

namespace Core.Scoring.Credit;

public interface ICreditInfoListener
{

  Task StoreCreditInfo(Pesel pesel, CreditInfo? creditInfo);
}
