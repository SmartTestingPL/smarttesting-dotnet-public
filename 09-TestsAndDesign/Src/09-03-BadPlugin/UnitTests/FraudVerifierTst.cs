using NUnit.Framework;

namespace UnitTests;

public class FraudVerifierTst
{
  /// <summary>
  /// Klasa testowa wyłamująca się poza schemat nazewniczy testów.
  /// W Javie to miało znaczenie i służyło zademonstrowaniu przełącznika
  /// do buildu który wywalał build, jeśli nie znalazł żadnego testu.
  /// W NUnit3 konwencja nazewnicza nie ma znaczenia, nie znalazłem też
  /// narzędzia pozwalającego zgłosić błąd, gdy nie ma żadnego testu.
  ///
  /// Można spróbować coś takiego zaimplementować ręcznie,
  /// sprawdzając wyjście z konsoli jakimś skryptem.
  /// Przykład jest w klasie Program w projekcie BuildScript.
  /// Nie polecam jednak tego rozwiązania, bo bywa dosyć kruche.
  /// </summary>
  [Test, Ignore("pada celowo")]
  public void ShouldMarkClientWithDebtAsFraud()
  {
    Assert.False(new FraudVerifier().IsFraud(new Client(true)));
  }
}

internal class FraudVerifier
{
  internal bool IsFraud(Client client)
  {
    return client.HasDebt;
  }
}

internal class Client
{
  internal readonly bool HasDebt;

  internal Client(bool hasDebt)
  {
    HasDebt = hasDebt;
  }
}