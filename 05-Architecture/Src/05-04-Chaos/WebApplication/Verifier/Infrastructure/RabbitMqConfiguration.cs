namespace WebApplication.Verifier.Infrastructure;

/// <summary>
/// Konfiguracja brokera wiadomości RabbitMq, wczytywana z appsettings.json
/// bądź innego źródła (np. zmiennych środowiskowych).
/// </summary>
public class RabbitMqConfiguration
{
  public string ConnectionString { get; set; } = default!;
}