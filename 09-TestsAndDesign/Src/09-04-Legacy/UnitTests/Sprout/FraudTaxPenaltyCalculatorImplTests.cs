using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.Sprout;

public class FraudTaxPenaltyCalculatorImplTests
{
  /// <summary>
  /// Test z wykorzystaniem sztucznej implementacji dostępu do bazy danych.
  /// </summary>
  [Test]
  public void ShouldCalculateTheTaxForFraudowski()
  {
    const int fraudowskiAmount = 100;
    var accessor = new FakeDatabaseAccessorImpl(fraudowskiAmount);
    var calculator = new _05_FraudTaxPenaltyCalculatorImpl(accessor);

    var tax = calculator.CalculateFraudTax("Fraudowski");

    tax.Should().Be(fraudowskiAmount * 100);
  }

  /// <summary>
  /// Test z wykorzystaniem sztucznej implementacji dostępu do bazy danych.
  /// Weryfikuje implementację z użyciem if / else.
  /// </summary>
  [Test]
  public void ShouldCalculateTheTaxForFraudowskiWithIfElse()
  {
    const int fraudowskiAmount = 100;
    var accessor = new FakeDatabaseAccessorImpl(fraudowskiAmount);
    var calculator = new _06_FraudTaxPenaltyCalculatorImplIfElse(accessor);

    var tax = calculator.CalculateFraudTax("Fraudowski");

    tax.Should().Be(fraudowskiAmount * 100 * 10);
  }

  /// <summary>
  /// Test z wykorzystaniem sztucznej implementacji dostępu do bazy danych.
  /// Weryfikuje implementację z użyciem klasy kiełkującej.
  /// </summary>
  [Test]
  public void ShouldCalculateTheTaxForFraudowskiWithSprout()
  {
    const int fraudowskiAmount = 100;
    var accessor = new FakeDatabaseAccessorImpl(fraudowskiAmount);
    var calculator = new _07_FraudTaxPenaltyCalculatorImplSprout(accessor);

    var tax = calculator.CalculateFraudTax("Fraudowski");

    tax.Should().Be(fraudowskiAmount * 100 * 20);
  }
}

/// <summary>
/// Kalkulator podatku dla oszustów. Nie mamy do niego testów.
/// </summary>
internal class _05_FraudTaxPenaltyCalculatorImpl
{
  private readonly DatabaseAccessorImpl _databaseImpl;

  internal _05_FraudTaxPenaltyCalculatorImpl(DatabaseAccessorImpl databaseImpl)
  {
    _databaseImpl = databaseImpl;
  }

  internal int CalculateFraudTax(string name)
  {
    var client = _databaseImpl.GetClientByName(name);
    if (client.Amount < 0)
    {
      // WARNING: Don't touch this
      // nobody knows why it should be -3 anymore
      // but nothing works if you change this
      return -3;
    }

    return CalculateTax(client.Amount);
  }

  private int CalculateTax(int amount)
  {
    return amount * 100;
  }
}

/// <summary>
/// Nowa funkcja systemu - dodajemy kod do nieprzetestowanego kodu.
/// </summary>
internal class _06_FraudTaxPenaltyCalculatorImplIfElse
{
  private readonly DatabaseAccessorImpl _databaseImpl;

  internal _06_FraudTaxPenaltyCalculatorImplIfElse(
    DatabaseAccessorImpl databaseImpl)
  {
    _databaseImpl = databaseImpl;
  }

  public int CalculateFraudTax(string name)
  {
    var client = _databaseImpl.GetClientByName(name);
    if (client.Amount < 0)
    {
      // WARNING: Don't touch this
      // nobody knows why it should be -3 anymore
      // but nothing works if you change this
      return -3;
    }

    var tax = CalculateTax(client.Amount);
    if (tax > 10)
    {
      return tax * 10;
    }

    return tax;
  }

  private int CalculateTax(int amount)
  {
    return amount * 100;
  }
}

/// <summary>
/// Klasa kiełkowania (sprout). Wywołamy kod, który został przetestowany.
/// Piszemy go poprzez TDD.
/// </summary>
internal class _07_FraudTaxPenaltyCalculatorImplSprout
{
  private readonly DatabaseAccessorImpl _databaseImpl;

  internal _07_FraudTaxPenaltyCalculatorImplSprout(DatabaseAccessorImpl databaseImpl)
  {
    _databaseImpl = databaseImpl;
  }

  internal int CalculateFraudTax(string name)
  {
    var client = _databaseImpl.GetClientByName(name);
    if (client.Amount < 0)
    {
      // WARNING: Don't touch this
      // nobody knows why it should be -3 anymore
      // but nothing works if you change this
      return -3;
    }

    var tax = CalculateTax(client.Amount);
    // chcemy obliczyć specjalny podatek
    return new SpecialTaxCalculator(tax).Calculate();
  }

  private int CalculateTax(int amount)
  {
    return amount * 100;
  }
}

internal class DatabaseAccessorImpl
{
  public virtual Client GetClientByName(string name)
  {
    return new Client(name, true, 100);
  }
}

internal class FakeDatabaseAccessorImpl : DatabaseAccessorImpl
{

  private readonly int _amount;

  internal FakeDatabaseAccessorImpl(int amount)
  {
    _amount = amount;
  }


  public override Client GetClientByName(string name)
  {
    return new Client("Fraudowski", true, _amount);
  }
}

internal class Client
{
  private readonly string _name;
  private readonly bool _hasDebt;
  internal readonly int Amount;

  internal Client(string name, bool hasDebt, int amount)
  {
    _name = name;
    _hasDebt = hasDebt;
    Amount = amount;
  }
}