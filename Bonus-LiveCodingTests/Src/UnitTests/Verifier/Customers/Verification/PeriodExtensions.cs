using NodaTime;

namespace UnitTests.Verifier.Customers.Verification;

public static class PeriodExtensions
{
  public static Period Years(this int count)
  {
    return Period.FromYears(count);
  }
}