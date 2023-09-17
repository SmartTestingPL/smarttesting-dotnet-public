using System;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

/// <summary>
/// Serwis procesujący przynawanie pożyczek w zależności
/// od typu pożyczki i obowiązujących promocji.
/// </summary>
public class LoanOrderService
{
  public LoanOrder StudentLoanOrder(Customer customer)
  {
    if (!customer.IsStudent)
    {
      throw new InvalidOperationException("Cannot order student loan if pl.smarttesting.customer is not a student.");
    }

    var now = Clocks.ZonedUtc.GetCurrentDate();
    var loanOrder = new LoanOrder(now, customer)
    {
      Type = LoanType.Student,
      Commission = 200
    };
    loanOrder.Promotions.Add(new Promotion("Student Promo", 10));
    return loanOrder;
  }
}