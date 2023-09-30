namespace WebApplication;

/// <summary>
/// Normalnie wczytalibyśmy tę konfigurację z appsettingsów,
/// ale nawet nie umieszczałem tam tej wartości bo z punktu
/// widzenia tego przykłądu nie ma to znaczenia.
/// Testy ustawiają swoją wartość w dodatkowej konfiguracji.
/// </summary>
public class RabbitMqConfiguration
{
  public string ConnectionString { get; set; } = default!;
}