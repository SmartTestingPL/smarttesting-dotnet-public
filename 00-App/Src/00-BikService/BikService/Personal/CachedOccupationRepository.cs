using Core.Scoring.domain;
using Core.Scoring.Personal;
using Microsoft.Extensions.Caching.Memory;

namespace BikService.Personal;

public class CachedOccupationRepository : IOccupationRepository
{
  private const string CacheKey = "occupationScores";
  private readonly OccupationRepositoryWatcher _repository;
  private readonly IMemoryCache _memoryCache;

  public CachedOccupationRepository(
    OccupationRepositoryWatcher repository,
    IMemoryCache memoryCache)
  {
    _repository = repository;
    _memoryCache = memoryCache;
  }

  public Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    var isValueInCache = _memoryCache.TryGetValue<Dictionary<PersonalInformation.Occupations?, Score>>(CacheKey, out var cachedScores);
    if (isValueInCache)
    {
      return cachedScores;
    }
    var occupationScores = _repository.GetOccupationScores();
    _memoryCache.Set(CacheKey, occupationScores);
    return occupationScores;
  }
}