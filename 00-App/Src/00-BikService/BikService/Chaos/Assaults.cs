namespace BikService.Chaos;

public class Assaults
{
  /// <summary>
  /// Przełącznik do włączania/wyłączania
  /// wstrzykiwania wyjątków
  /// </summary>
  public bool EnableExceptionAssault = false;

  /// <summary>
  /// Konfigurację przechowuję w instancji, żeby
  /// łatwiej było ją czyścić (patrz <see cref="Clear"/>)
  /// </summary>
  public static Assaults Config { get; private set; } = new();

  /// <summary>
  /// Czyści konfigurację ataków
  /// </summary>
  public static void Clear()
  {
    Config = new Assaults();
  }

}