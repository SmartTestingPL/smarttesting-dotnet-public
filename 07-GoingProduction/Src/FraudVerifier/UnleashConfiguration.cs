using System;

namespace FraudVerifier;

/// <summary>
/// Konfiguracja połączenia z Unleash:
/// - nazwa naszej aplikacji
/// - URI do połączenia z instancją Unleash
/// </summary>
public class UnleashConfiguration
{
  public string? AppName { get; set; }
  public Uri? UnleashApi { get; set; }
}