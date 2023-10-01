using NodaTime;

namespace IntegrationTests;

public static class PeriodExtensions
{
  public static Period Years(this int count)
  {
    return Period.FromYears(count);
  }
}