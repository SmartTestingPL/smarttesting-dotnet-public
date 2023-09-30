using System;
using System.Linq;
using NodaTime;
using ProductionCode.Lib;
using ProductionCode.Loans.Validation;
using ProductionCode.Orders;

namespace ProductionCode.Loans;

public class Loan
{
  public Loan(LocalDate loanOpenedDate, LoanOrder loanOrder, int numberOfInstallments)
  {
    LoanOpenedDate = loanOpenedDate;
    Amount = CalculateLoanAmount(loanOrder);
    NumberOfInstallments = numberOfInstallments;
    InstallmentAmount = Amount.Divide(numberOfInstallments, 2, MidpointRounding.ToEven);
    Guid = Guid.NewGuid();
  }

  public Loan(LoanOrder loanOrder, int numberOfInstallments) 
    : this(Clocks.ZonedUtc.GetCurrentDate(), loanOrder, numberOfInstallments)
  {
  }

  private decimal CalculateLoanAmount(LoanOrder loanOrder)
  {
    ValidateElement(loanOrder.Amount);
    ValidateElement(loanOrder.InterestRate);
    ValidateElement(loanOrder.Commission);
    var interestFactor =
      new decimal(1).Add(loanOrder.InterestRate!.Value.Divide(100, 2, MidpointRounding.AwayFromZero));
    var baseAmount = loanOrder.Amount!.Value.Multiply(interestFactor).Add(loanOrder.Commission!.Value);
    return ApplyPromotionDiscounts(loanOrder, baseAmount);
  }

  private decimal ApplyPromotionDiscounts(LoanOrder loanOrder, decimal baseAmount)
  {
    var discountSum = loanOrder.Promotions
      .Select(p => p.Discount)
      .Aggregate(decimal.Zero, (d1, d2) => d1.Add(d2));
    var fifteenPercentOfBaseSum =
      baseAmount.Multiply(15).Divide(100, MidpointRounding.ToEven);
    if (fifteenPercentOfBaseSum.CompareTo(discountSum) <= 0)
    {
      return baseAmount.Subtract(fifteenPercentOfBaseSum);
    }

    return baseAmount.Subtract(discountSum);
  }

  private void ValidateElement(decimal? elementAmount)
  {
    if (elementAmount == null || elementAmount.Value.CompareTo(decimal.One) < 0)
    {
      throw new LoanValidationException();
    }
  }

  public decimal InstallmentAmount { get; }
  public Guid Guid { get; }
  public LocalDate LoanOpenedDate { get; }
  public int NumberOfInstallments { get; }
  public decimal Amount { get; }
}