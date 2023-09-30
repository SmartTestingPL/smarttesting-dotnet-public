using System;
using ProductionCode.Customers;
using ProductionCode.Db;
using ProductionCode.Lib;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

/// <summary>
/// Serwis procesujący przynawanie pożyczek
/// w zależności od typu pożyczki i obowiązujących promocji.
/// </summary>
public class LoanOrderService
{
  private readonly IMongoDbAccessor _mongoDbAccessor;
  private readonly IPostgresAccessor _postgresDbAccessor;

  public LoanOrderService(
    IPostgresAccessor postgresDbAccessor, 
    IMongoDbAccessor mongoDbAccessor)
  {
    _postgresDbAccessor = postgresDbAccessor;
    _mongoDbAccessor = mongoDbAccessor;
  }

  public LoanOrder StudentLoanOrder(Customer customer)
  {
    if (!customer.IsStudent)
    {
      throw new InvalidOperationException(
        "Cannot order student loan if customer is not a student.");
    }

    var now = Clocks.ZonedUtc.GetCurrentDate();
    var loanOrder = new LoanOrder(now, customer, 2000, 5, 200)
    {
      Type = LoanType.Student,
    };
    var discount = _mongoDbAccessor.GetPromotionDiscount("Student Promo");
    loanOrder.Promotions.Add(new Promotion("Student Promo", discount));

    _postgresDbAccessor.UpdatePromotionStatistics("Student Promo");
    return loanOrder;
  }
}