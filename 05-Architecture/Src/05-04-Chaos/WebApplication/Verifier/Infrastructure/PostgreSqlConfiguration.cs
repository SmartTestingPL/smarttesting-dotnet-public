namespace WebApplication.Verifier.Infrastructure;

/// <summary>
/// Konfiguracja bazy danych PostgreSQL wczytywana z appsettings.json
/// bądź innego źródła (np. zmiennych środowiskowych).
/// </summary>
public class PostgreSqlConfiguration
{
  public string ConnectionString { get; set; } = default!;
}