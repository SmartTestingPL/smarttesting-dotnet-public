using NodaTime;

namespace Core.Lib;

public static class Clocks
{
  public static ZonedClock ZonedUtc { get; }
    = new ZonedClock(SystemClock.Instance, DateTimeZone.Utc, CalendarSystem.Iso);
}