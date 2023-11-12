using System;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.StaticMethod;

public class _17_FraudVerifierTests
{
  /// <summary>
  /// Test się wywala, gdyż wywołanie `isFraud` wywoła połączenie do bazy danych.
  /// Nie wierzysz? Zakomentuj <see cref="IgnoreAttribute"/> i sprawdź sam!
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldMarkClientWithDebtAsFraud()
  {
    var verifier = new _18_FraudVerifier();

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  /// <summary>
  /// Test wykorzystujący możliwość podmiany globalnej, statycznej
  /// instancji produkcyjnej na wersję testową, która zwraca wartość
  /// ustawioną na sztywno. Sprzątanie po tej podmianie jest w TearDown.
  /// </summary>
  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithStatic()
  {
    _20_DatabaseAccessorImplWithSetter.Instance = new _21_FakeDatabaseAccessor();

    var verifier = new FraudVerifierForSetter();

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  /// <summary>
  /// Ważne, żeby po sobie posprzątać!
  /// </summary>
  [TearDown]
  public void Clean()
  {
    _20_DatabaseAccessorImplWithSetter.Reset();
  }

}

/// <summary>
/// Przykład implementacji wołającej singleton
/// <see cref="_19_DatabaseAccessorImpl"/>.
/// </summary>
internal class _18_FraudVerifier
{
  internal bool IsFraud(string name)
  {
    var client = _19_DatabaseAccessorImpl.Instance.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class FraudVerifierForSetter
{
  internal bool IsFraud(string name)
  {
    var client = _20_DatabaseAccessorImplWithSetter.Instance.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class _19_DatabaseAccessorImpl
{
  protected _19_DatabaseAccessorImpl()
  {

  }

  public static _19_DatabaseAccessorImpl Instance { get; } 
    = new _19_DatabaseAccessorImpl();

  public Client GetClientByName(string name)
  {
    var client = PerformLongRunningTask(name);
    Console.WriteLine(client.Name);
    DoSomeAdditionalWork(client);
    return client;
  }

  private static void DoSomeAdditionalWork(Client client)
  {
    Console.WriteLine("Additional work done");
  }

  private static Client PerformLongRunningTask(string name)
  {
    throw new InvalidOperationException("Can't connect to the database");
  }
}

internal class _21_FakeDatabaseAccessor : _20_DatabaseAccessorImplWithSetter
{
  public override Client GetClientByName(string name)
  {
    return new Client("Fraudowski", true);
  }
}

internal class _20_DatabaseAccessorImplWithSetter
{
  protected _20_DatabaseAccessorImplWithSetter()
  {

  }

  public static _20_DatabaseAccessorImplWithSetter Instance
  {
    get; 
    set; //Ustawiamy wartość testową.
  }
    = new _20_DatabaseAccessorImplWithSetter();

  /// <summary>
  /// Resetujemy aktualną wartość do wartości produkcyjnej.
  /// </summary>
  public static void Reset()
  {
    Instance = new _20_DatabaseAccessorImplWithSetter();
  }

  public virtual Client GetClientByName(string name)
  {
    var client = PerformLongRunningTask(name);
    Console.WriteLine(client.Name);
    DoSomeAdditionalWork(client);
    return client;
  }

  private static void DoSomeAdditionalWork(Client client)
  {
    Console.WriteLine("Additional work done");
  }

  private static Client PerformLongRunningTask(string name)
  {
    throw new InvalidOperationException("Can't connect to the database");
  }
}

internal class Client
{
  internal readonly string Name;
  internal readonly bool HasDebt;

  internal Client(string name, bool hasDebt)
  {
    Name = name;
    HasDebt = hasDebt;
  }
}