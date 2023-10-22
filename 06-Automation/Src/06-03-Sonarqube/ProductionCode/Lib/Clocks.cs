using NodaTime;

namespace ProductionCode.Lib;

public static class Clocks
{
  public static ZonedClock ZonedUtc { get; } = new(SystemClock.Instance, DateTimeZone.Utc, CalendarSystem.Iso);
}