using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.PassingNull;

internal class _15_FraudVerifierTests
{
  /// <summary>
  /// Przykład testu, gdzie zakładamy, że nie musimy tworzyć wszystkich
  /// obiektów i podmieniamy je nullem. Jeśli zależność jest wymagana
  /// - test nam się wywali.
  /// </summary>
  [Test]
  public void ShouldMarkClientWithDebtAsFraud()
  {
    var verifier = new _16_FraudVerifier(null, null, new DatabaseAccessorImpl());

    verifier.IsFraud("Fraudowski").Should().BeTrue();
  }

  /// <summary>
  /// Przykład testu, gdzie zakładamy, że nie musimy tworzyć wszystkich
  /// obiektów i podmieniamy je nullem. Niestety nie trafiamy i leci
  /// nam NullPointerException, gdyż dani współpracownicy byli wymagani.
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldCalculatePenaltyWhenFraudAppliesForALoan()
  {
    var verifier = new _16_FraudVerifier(new PenaltyCalculatorImpl(), null, null);

    var penalty = verifier.CalculateFraudPenalty("Fraudowski");

    penalty.Should().BeGreaterThan(0);
  }

  /// <summary>
  /// Wygląda na to, że musimy przekazać jeszcze
  /// <see cref="TaxHistoryRetrieverImpl"/>.
  /// </summary>
  [Test]
  public void ShouldCalculatePenaltyWhenFraudAppliesForALoanWithBothDeps()
  {
    var verifier = new _16_FraudVerifier(
      new PenaltyCalculatorImpl(), 
      new TaxHistoryRetrieverImpl(), 
      null);

    var penalty = verifier.CalculateFraudPenalty("Fraudowski");

    penalty.Should().BeGreaterThan(0L);
  }
}

/// <summary>
/// Implementacja zawierająca dużo zależności, skomplikowany, długi kod.
/// </summary>
internal class _16_FraudVerifier
{
  private readonly PenaltyCalculatorImpl _penalty;
  private readonly TaxHistoryRetrieverImpl _history;
  private readonly DatabaseAccessorImpl _accessor;

  internal _16_FraudVerifier(
    PenaltyCalculatorImpl penalty, 
    TaxHistoryRetrieverImpl history,
    DatabaseAccessorImpl accessor)
  {
    _penalty = penalty;
    _history = history;
    _accessor = accessor;
  }

  public long CalculateFraudPenalty(string name)
  {
    // 5 000 linijek kodu dalej...

    // set client history to false, otherwise it won't work
    var lastRevenue = _history.ReturnLastRevenue(new Client(name, false));
    // set client history to true, otherwise it won't work
    var penalty = _penalty.CalculatePenalty(new Client(name, true));
    return lastRevenue / 50 + penalty;
  }

  public bool IsFraud(string name)
  {
    // 7 000 linijek kodu dalej ...

    var client = _accessor.GetClientByName(name);
    return client.HasDebt;
  }
}

internal class PenaltyCalculatorImpl
{
  internal long CalculatePenalty(Client client)
  {
    return 100L;
  }
}

internal class TaxHistoryRetrieverImpl
{
  internal long ReturnLastRevenue(Client client)
  {
    return 150;
  }
}

internal class DatabaseAccessorImpl
{
  public Client GetClientByName(string name)
  {
    return new Client("Fraudowski", true);
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