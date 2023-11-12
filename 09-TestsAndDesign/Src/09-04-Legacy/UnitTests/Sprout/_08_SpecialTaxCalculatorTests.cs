using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.Sprout;

public class _08_SpecialTaxCalculatorTests
{
  [Test]
  public void ShouldNotApplySpecialTaxWhenAmountNotReachingThreshold()
  {
    const int initialAmount = 8;
    var calculator = new SpecialTaxCalculator(initialAmount);

    calculator.Calculate().Should().Be(initialAmount);
  }

  [Test]
  public void ShouldApplySpecialTaxWhenAmountReachesThreshold()
  {
    const int initialAmount = 25;
    var calculator = new SpecialTaxCalculator(initialAmount);

    calculator.Calculate().Should().Be(500);
  }
}

internal class SpecialTaxCalculator
{
  private const int AmountThreshold = 10;
  private const int TaxMultiplier = 20;
  private readonly int _amount;

  internal SpecialTaxCalculator(int amount)
  {
    _amount = amount;
  }

  internal int Calculate()
  {
    if (_amount <= AmountThreshold)
    {
      return _amount;
    }

    return _amount * TaxMultiplier;
  }
}