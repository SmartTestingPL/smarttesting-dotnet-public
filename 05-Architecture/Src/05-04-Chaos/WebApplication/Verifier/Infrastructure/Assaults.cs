using System;
using WebApplication.Verifier.Model;

namespace WebApplication.Verifier.Infrastructure;

/// <summary>
/// Prymitywna klasa przechowująca aktualne ustawienia ataków.
/// Użyta żeby uprościć przykład.
/// Normalnie statyczny zmienny stan to prawie zawsze zły pomysł.
/// </summary>
public class Assaults
{
  private readonly Random _random = new Random();
    
  /// <summary>
  /// Przełącznik do włączania/wyłączania
  /// wstrzykiwania wyjątków
  /// </summary>
  public bool EnableExceptionAssault = false;

  /// <summary>
  /// Przełącznik do włączania/wyłączania
  /// wstrzykiwania opóźnień
  /// </summary>
  public bool EnableLatencyAssault = false;

  /// <summary>
  /// Początek zakresu z którego losowane będzie opóźnienie
  /// w milisekundach.
  /// </summary>
  public int LatencyRangeStart { private get; set; } = 1000;

  /// <summary>
  /// Koniec zakresu z którego losowane będzie opóźnienie
  /// w milisekundach.
  /// </summary>
  public int LatencyRangeEnd { private get; set; } = 3000;

  /// <summary>
  /// Konfigurację przechowuję w instancji, żeby
  /// łatwiej było ją czyścić (patrz <see cref="Clear"/>)
  /// </summary>
  public static Assaults Config { get; private set; } = new Assaults();

  /// <summary>
  /// Czyści konfigurację ataków
  /// </summary>
  public static void Clear()
  {
    Config = new Assaults();
  }

  public int RandomLatencyFromRange()
  {
    return _random.Next(LatencyRangeStart, LatencyRangeEnd);
  }
}