using System.Collections.Concurrent;
using Core.Scoring.Credit;
using Core.Scoring.domain;

namespace BikService.Credit;

public class DevCreditInfoRepository : ICreditInfoRepository, ICreditInfoRepositoryForInitialization
{
  private readonly ConcurrentDictionary<Pesel, CreditInfo> _db = new();
  public async Task<CreditInfo?> FindCreditInfo(Pesel pesel)
  {
    _db.TryGetValue(pesel, out var value);
    return await Task.FromResult(value);
  }

  public async Task<CreditInfo> SaveCreditInfo(Pesel pesel, CreditInfo creditInfo)
  {
    _db[pesel] = creditInfo;
    return await Task.FromResult(creditInfo);
  }

  public async Task Save(CreditInfoDocument creditInfoDocument)
  {
    _db[creditInfoDocument.Pesel] = creditInfoDocument.CreditInfo;
    await Task.CompletedTask;
  }

  public async Task Clear()
  {
    _db.Clear();
    await Task.CompletedTask;
  }
}
