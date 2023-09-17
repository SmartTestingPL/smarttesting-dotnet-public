using Core.Scoring.Credit;
using Core.Scoring.domain;
using Microsoft.Extensions.Caching.Memory;

namespace BikService.Credit;

public class CachedCreditInfoRepository : ICreditInfoRepository, ICreditInfoRepositoryForInitialization
{
  private readonly CreditInfoRepositoryWatcher _repository;
  private readonly IMemoryCache _cache;

  public CachedCreditInfoRepository(CreditInfoRepositoryWatcher repository, IMemoryCache memoryCache)
  {
    _cache = memoryCache;
    _repository = repository;
  }

  public async Task<CreditInfo?> FindCreditInfo(Pesel pesel)
  {
    if (_cache.TryGetValue<CreditInfo>(pesel, out var value))
    {
      return await Task.FromResult(value);
    }
    else
    {
      var creditInfo = await _repository.FindCreditInfo(pesel);
      _cache.Set(pesel, creditInfo, TimeSpan.FromMinutes(1));
      return creditInfo;
    }
  }

  public async Task<CreditInfo> SaveCreditInfo(Pesel pesel, CreditInfo creditInfo)
  {
    var info = await _repository.SaveCreditInfo(pesel, creditInfo);
    _cache.Set(pesel, info, TimeSpan.FromMinutes(1));
    return info;
  }

  public async Task Save(CreditInfoDocument creditInfoDocument)
  {
    await _repository.Save(creditInfoDocument);
  }

  public async Task Clear()
  {
    await _repository.Clear();
  }
}