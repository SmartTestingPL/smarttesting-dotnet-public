using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests;

/// <summary>
/// Klasa testowa pokazująca jak stanowość może pokrzyżować nam plany
/// w powtarzalnych wynikach testów.
/// 
/// Najpierw zakomentuj <see cref="IgnoreAttribute"/>, żeby wszystkie
/// testy się uruchomiły.
/// 
/// Następnie uruchom testy kilkukrotnie - zobaczysz, że czasami przechodzą,
/// a czasami nie. W czym problem?
/// </summary>
class _04_PotentialFraudServiceTests
{
  /// <summary>
  /// Test ten oczekuje, że zawsze uruchomi się pierwszy. Dlatego oczekuje,
  /// że w cacheu będzie jeden wynik. Dla przypomnienia, cache jest
  /// współdzielony przez wszystkie testy, ponieważ jest statyczny.
  /// 
  /// W momencie uruchomienia testów w innej kolejności, inne testy
  /// też dodają wpisy do cachea. Zatem nie ma możliwości, żeby rozmiar
  /// cachea wynosił 1.
  /// </summary>
  [Ignore("oczekuje, że uruchomi się pierwszy")]
  [Test]
  public void ShouldCountPotentialFrauds()
  {
    var cache = new PotentialFraudCache();
    var service = new PotentialFraudService(cache);

    service.SetFraud("Kowalski");

    cache.FraudsSize.Should().Be(1);
  }

  /// <summary>
  /// Przykład testu, który weryfikuje czy udało nam się dodać wpis do cachea.
  /// Zwiększa rozmiar cachea o 1. Gdy ten test zostanie uruchomiony przed
  /// ShouldCountPotentialFrauds - wspomniany test się wywali.
  /// </summary>
  [Test]
  public void ShouldSetPotentialFraud()
  {
    var cache = new PotentialFraudCache();
    var service = new PotentialFraudService(cache);

    service.SetFraud("Oszustowski");

    cache.Fraud("Oszustowski").Should().NotBe(null);
  }

  /// <summary>
  /// Potencjalne rozwiązanie problemu wspóldzielonego stanu. Najpierw
  /// zapisujemy stan wejściowy - jaki był rozmiar cachea. Dodajemy wpis
  /// do cachea i sprawdzamy czy udało się go dodać i czy rozmiar jest
  /// większy niż był.
  /// 
  /// W przypadku uruchomienia wielu testów równolegle, sam fakt weryfikacji
  /// rozmiaru jest niedostateczny, gdyż inny test mógł zwiększyć rozmiar
  /// cachea. Koniecznym jest zweryfikowanie, że istnieje w cacheu wpis
  /// dot. Kradzieja.
  /// 
  /// BONUS: Jeśli inny test weryfikował usunięcie wpisu z cachea, to asercja
  /// na rozmiar może nam się wysypać. Należy rozważyć, czy nie jest wystarczającym
  /// zweryfikowanie tylko obecności Kradzieja w cacheu!
  /// </summary>
  [Test]
  //[Repeat(100)] jeśli odkomentujemy, ten test zacznie padać
  public void ShouldStorePotentialFraud()
  {
    var cache = new PotentialFraudCache();
    var service = new PotentialFraudService(cache);
    var initialSize = cache.FraudsSize;

    service.SetFraud("Kradziej");

    cache.FraudsSize.Should().BeGreaterThan(initialSize);
    cache.Fraud("Kradziej").Should().NotBe(null);
  }

}

class PotentialFraudCache
{
  /// <summary>
  /// Stan współdzielony między instancjami. Problemy? Np. używamy
  /// niebezpiecznej dla wątków implementacji słownika.
  /// </summary>
  static readonly Dictionary<string, PotentialFraud> Cache
    = new Dictionary<string, PotentialFraud>();

  internal PotentialFraud Fraud(string name)
  {
    return Cache[name];
  }

  internal void Put(PotentialFraud fraud)
  {
    Cache[fraud.Name] = fraud;
  }

  internal int FraudsSize => Cache.Count;
}

/// <summary>
/// Serwis aplikacyjny opakowujący wywołania do cachea.
/// </summary>
class PotentialFraudService
{
  private readonly PotentialFraudCache _cache;

  internal PotentialFraudService(PotentialFraudCache cache)
  {
    _cache = cache;
  }

  internal void SetFraud(string name)
  {
    _cache.Put(new PotentialFraud(name));
  }
}

/// <summary>
/// Struktura reprezentująca potencjalnego oszusta.
/// </summary>
class PotentialFraud
{
  internal readonly string Name;

  internal PotentialFraud(string name)
  {
    Name = name;
  }
}