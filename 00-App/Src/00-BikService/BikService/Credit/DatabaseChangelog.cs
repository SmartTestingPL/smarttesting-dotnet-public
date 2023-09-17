using Core.Scoring.Credit;
using Core.Scoring.domain;

namespace BikService.Credit;

/// <summary>
/// Dane testowe do bazy danych - ułatwienie dla uczestników szkolenia
/// </summary>
public class DatabaseChangelog
{
  private readonly ICreditInfoRepositoryForInitialization _repo;

  public DatabaseChangelog(ICreditInfoRepositoryForInitialization repo)
  {
    _repo = repo;
  }

  public async Task Change()
  {
    await _repo.Clear();
    await _repo.Save(CreditInfo1());
    await _repo.Save(CreditInfo2());
    await _repo.Save(CreditInfo3());
    await _repo.Save(CreditInfo4());
    await _repo.Save(CreditInfo5());
  }

  private CreditInfoDocument CreditInfo1()
  {
    var creditInfo = new CreditInfo(decimal.Parse("100"), decimal.Parse("200"), CreditInfo.DebtPaymentHistoryStatus.NotASingleUnpaidInstallment);
    var pesel = new Pesel("89050193724");
    return new CreditInfoDocument(creditInfo, pesel);
  }

  private CreditInfoDocument CreditInfo2()
  {
    var creditInfo = new CreditInfo(decimal.Parse("500"), decimal.Parse("1000"), CreditInfo.DebtPaymentHistoryStatus.IndividualUnpaidInstallments);
    var pesel = new Pesel("56020172634");
    return new CreditInfoDocument(creditInfo, pesel);
  }

  private CreditInfoDocument CreditInfo3()
  {
    var creditInfo = new CreditInfo(decimal.Parse("1000"), decimal.Parse("2000"), CreditInfo.DebtPaymentHistoryStatus.IndividualUnpaidInstallments);
    var pesel = new Pesel("79061573376");
    return new CreditInfoDocument(creditInfo, pesel);
  }

  private CreditInfoDocument CreditInfo4()
  {
    var creditInfo = new CreditInfo(decimal.Parse("5000"), decimal.Parse("7000"), CreditInfo.DebtPaymentHistoryStatus.MultipleUnpaidInstallments);
    var pesel = new Pesel("64091148892");
    return new CreditInfoDocument(creditInfo, pesel);
  }

  private CreditInfoDocument CreditInfo5()
  {
    var creditInfo = new CreditInfo(decimal.Parse("10000"), decimal.Parse("20000"), CreditInfo.DebtPaymentHistoryStatus.NotASinglePaidInstallment);
    var pesel = new Pesel("63081514479");
    return new CreditInfoDocument(creditInfo, pesel);
  }
}