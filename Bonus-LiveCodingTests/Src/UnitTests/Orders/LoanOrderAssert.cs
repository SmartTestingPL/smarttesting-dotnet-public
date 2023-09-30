using FluentAssertions;
using ProductionCode.Lib;
using ProductionCode.Orders;

namespace UnitTests.Orders;

/// <summary>
/// Przykład zastosowania wzorca AssertObject.
/// </summary>
public class LoanOrderAssert
{
  private readonly LoanOrder _loanOrder;

  public LoanOrderAssert(LoanOrder loanOrder)
  {
    _loanOrder = loanOrder;
  }

  public LoanOrderAssert BeRegisteredToday()
  {
    _loanOrder.OrderDate.Should().Be(Clocks.ZonedUtc.GetCurrentDate());
    return this;
  }

  public LoanOrderAssert HavePromotion(string promotionName)
  {
    _loanOrder.Promotions.Should().ContainSingle(promotion => promotion.Name == promotionName);
    return this;
  }

  public LoanOrderAssert HaveOnlyOnePromotion()
  {
    HavePromotionNumber(1);
    return this;
  }

  private LoanOrderAssert HavePromotionNumber(int number)
  {
    _loanOrder.Promotions.Should().HaveCount(number);
    return this;
  }

  public LoanOrderAssert HaveOnFirstPromotionDiscountValueOf(decimal number)
  {
    _loanOrder.Promotions[0].Discount.Should().Be(number);
    return this;
  }

  public LoanOrderAssert BeCorrect()
  {
    return BeRegisteredToday()
      .HavePromotion("Student Promo")
      .HaveOnlyOnePromotion()
      .HaveOnFirstPromotionDiscountValueOf(10);
  }
}

/// <summary>
/// Metoda rozszerzająca pozwalająca użyć składni np. value.Should().BeCorrect()
/// dla LoanOrderAssert
/// </summary>
public static class LoadAssertExtensions
{
  public static LoanOrderAssert Should(this LoanOrder loanOrder)
  {
    return new LoanOrderAssert(loanOrder);
  }
}