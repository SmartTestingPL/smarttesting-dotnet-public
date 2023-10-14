namespace WebApplication.Verifier.Model;

/// <summary>
/// Struktura danych do zdalnej konfiguracji
/// zakresu wstrzykiwanego opóźnienia
/// </summary>
public class LatencyRange
{
  public LatencyRange(int start, int end)
  {
    Start = start;
    End = end;
  }

  public int Start { get; }
  public int End { get; }
}