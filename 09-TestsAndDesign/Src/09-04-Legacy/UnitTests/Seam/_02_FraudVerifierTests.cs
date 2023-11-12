using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.Seam;

public class _02_FraudVerifierTests
{
  /// <summary>
  /// Przykład próby napisania testu do istniejącej klasy
  /// łączącej się z bazą danych.
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldMarkClientWithDebtAsFraud()
  {
    var accessor = new _03_DatabaseAccessorImpl();
    var verifier = new FraudVerifier(accessor);

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  /// <summary>
  /// Przykład testu z wykorzystaniem szwu (seam).
  /// </summary>
  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithSeam()
  {
    _03_DatabaseAccessorImpl accessor = new _04_FakeDatabaseAccessor();
    var verifier = new FraudVerifier(accessor);

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  [Ignore("")]
  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithSeamLogicInConstructor()
  {
    var verifier = new _09_FraudVerifierLogicInConstructor();

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  [Ignore("")]
  [Test]
  public void ShouldCreateAnInstanceOfFraudVerifier()
  {
    new _09_FraudVerifierLogicInConstructor();
  }

  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithAMock()
  {
    var databaseAccessor =
      Substitute.ForPartsOf<_10_DatabaseAccessorImplWithLogicInTheConstructor>("fake");
    databaseAccessor.GetClientByName(Arg.Any<string>()).Returns(new Client("Fraudowski", true));
    var verifier =
      new _11_FraudVerifierLogicInConstructorExtractLogic(databaseAccessor);

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithAnExtractedInterface()
  {
    var verifier = new _13_FraudVerifierWithInterface(new _10_FakeDatabaseAccessorWithInterface());

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  [Test]
  public void ShouldMarkClientWithDebtAsFraudWithSeamInterface()
  {
    _12_IDatabaseAccessor accessor = new _10_FakeDatabaseAccessorWithInterface();
    var verifier = new _13_FraudVerifierWithInterface(accessor);

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }
}

internal class FraudVerifier
{
  private readonly _03_DatabaseAccessorImpl _impl;

  internal FraudVerifier(_03_DatabaseAccessorImpl impl)
  {
    _impl = impl;
  }

  internal bool IsFraud(string name)
  {
    var client = _impl.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class _09_FraudVerifierLogicInConstructor
{

  private readonly _10_DatabaseAccessorImplWithLogicInTheConstructor _impl;

  internal _09_FraudVerifierLogicInConstructor()
  {
    _impl = new _10_DatabaseAccessorImplWithLogicInTheConstructor();
  }

  internal bool IsFraud(string name)
  {
    var client = _impl.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class _11_FraudVerifierLogicInConstructorExtractLogic
{

  private readonly _10_DatabaseAccessorImplWithLogicInTheConstructor _impl;

  internal _11_FraudVerifierLogicInConstructorExtractLogic()
  {
    _impl = new _10_DatabaseAccessorImplWithLogicInTheConstructor();
  }

  internal _11_FraudVerifierLogicInConstructorExtractLogic(_10_DatabaseAccessorImplWithLogicInTheConstructor impl)
  {
    _impl = impl;
  }

  internal bool IsFraud(string name)
  {
    var client = _impl.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class _03_DatabaseAccessorImpl
{
  public virtual Client GetClientByName(string name)
  {
    var client = PerformLongRunningTask(name);
    Console.WriteLine(client.Name);
    DoSomeAdditionalWork(client);
    return client;
  }

  private void DoSomeAdditionalWork(Client client)
  {
    Console.WriteLine("Additional work done");
  }

  private Client PerformLongRunningTask(string name)
  {
    throw new InvalidOperationException("Can't connect to the database");
  }
}

public class _10_DatabaseAccessorImplWithLogicInTheConstructor
{
  public _10_DatabaseAccessorImplWithLogicInTheConstructor()
  {
    ConnectToTheDatabase();
  }

  public _10_DatabaseAccessorImplWithLogicInTheConstructor(string fake)
  {
    //NSubstitute nie potrafi utworzyć mocka bez wołania konstruktora.
    //Stąd ten fałszywy konstruktor.
  }

  private void ConnectToTheDatabase()
  {
    throw new InvalidOperationException("Can't connect to the database");
  }

  public virtual Client GetClientByName(string name)
  {
    var client = PerformLongRunningTask(name);
    Console.WriteLine(client.Name);
    DoSomeAdditionalWork(client);
    return client;
  }

  private void DoSomeAdditionalWork(Client client)
  {
    Console.WriteLine("Additional work done");
  }

  private Client PerformLongRunningTask(string name)
  {
    throw new InvalidOperationException("Can't connect to the database");
  }
}

public class Client
{
  internal readonly string Name;
  internal readonly bool HasDebt;

  internal Client(string name, bool hasDebt)
  {
    Name = name;
    HasDebt = hasDebt;
  }
}

internal class _04_FakeDatabaseAccessor : _03_DatabaseAccessorImpl
{
  /// <summary>
  /// Nasz szew (seam)! Nadpisujemy problematyczną metodę
  /// bez zmiany kodu produkcyjnego.
  /// </summary>
  public override Client GetClientByName(string name)
  {
    return new Client("Fraudowski", true);
  }
}

internal class _10_FakeDatabaseAccessorWithInterface : _12_IDatabaseAccessor
{
  public Client GetClientByName(string name)
  {
    return new Client("Fraudowski", true);
  }
}

internal interface _12_IDatabaseAccessor
{
  Client GetClientByName(string name);
}

internal class DatabaseAccessorImplWithInterface : _12_IDatabaseAccessor
{
  internal DatabaseAccessorImplWithInterface()
  {
    ConnectToTheDatabase();
  }

  public Client GetClientByName(string name)
  {
    var client = PerformLongRunningTask(name);
    Console.WriteLine(client.Name);
    DoSomeAdditionalWork(client);
    return client;
  }

  private void ConnectToTheDatabase()
  {
    throw new InvalidOperationException("Can't connect to the database");
  }

  private void DoSomeAdditionalWork(Client client)
  {
    Console.WriteLine("Additional work done");
  }

  private Client PerformLongRunningTask(string name)
  {
    throw new InvalidOperationException("Can't connect to the database");
  }
}

internal class _13_FraudVerifierWithInterface
{
  private readonly _12_IDatabaseAccessor _accessor;

  internal _13_FraudVerifierWithInterface(_12_IDatabaseAccessor accessor)
  {
    _accessor = accessor;
  }

  internal bool IsFraud(string name)
  {
    var client = _accessor.GetClientByName(name);
    return client.HasDebt;
  }
}