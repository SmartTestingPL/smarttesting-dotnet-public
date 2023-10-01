using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ProductionCode.Lib;
using ProductionCode.Loans;
using ProductionCode.Orders;
using static UnitTests.Loans.LoanTestUtils;

namespace UnitTests.Loans;

public class LoanTest
{
  private const decimal Amount = 3000;
  private const decimal InterestRate = 5;
  private const decimal Commission = 200;

  [Test]
  public void ShouldCreateLoan()
  {
//		var loanOrder = ALoanOrder(3000, 5, 200);
    var loanOrder = ALoanOrder(Amount, InterestRate, Commission);

    var loan = new Loan(loanOrder, 6);

    loan.LoanOpenedDate.Should().Be(Clocks.ZonedUtc.GetCurrentDate());
    loan.NumberOfInstallments.Should().Be(6);
    loan.Amount.Should().Be(decimal.Parse("3350.00", CultureInfo.InvariantCulture));
  }

  [Test]
  public void ShouldCalculateInstallmentAmount()
  {
    var loanOrder = ALoanOrder(Amount, InterestRate, Commission);

    var loanInstallment = new Loan(loanOrder, 6).InstallmentAmount;

    loanInstallment.Should().Be(decimal.Parse("558.33", CultureInfo.InvariantCulture));
  }

  [Test]
  public void ShouldApplyPromotionDiscount()
  {
    var loanOrder = ALoanOrder(Amount, InterestRate, Commission, new Promotion("Test 10", 10),
      new Promotion("test 20", 20));

    var loan = new Loan(loanOrder, 6);

    loan.Amount.Should().Be(decimal.Parse("3320.00", CultureInfo.InvariantCulture));
    loan.InstallmentAmount.Should().Be(decimal.Parse("553.33", CultureInfo.InvariantCulture));
  }

  [Test]
  public void ShouldApplyFixedDiscountIfPromotionDiscountSumHigherThanThreshold()
  {
    var loanOrder = ALoanOrder(
      2000, 5, 300, 
      new Promotion("61", 61), 
      new Promotion("300", 300));

    // Base amount: 2400
    var loanAmount = new Loan(loanOrder, 6).Amount;

    loanAmount.Should().Be(decimal.Parse("2040.00", CultureInfo.InvariantCulture));
  }
}