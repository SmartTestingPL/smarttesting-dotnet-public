using System;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

public interface IPostgresAccessor
{
  void UpdatePromotionStatistics(string promotionName);

  void UpdatePromotionDiscount(string promotionName, decimal newDiscount);
}

public interface IMongoDbAccessor
{
  decimal GetPromotionDiscount(string promotionName);
}

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
        "Cannot order student loan " +
        "if pl.smarttesting.customer is not a student.");
    }

    var now = Clocks.ZonedUtc.GetCurrentDate();
    var loanOrder = new LoanOrder(now, customer)
    {
      Type = LoanType.Student,
      Commission = decimal.Parse("200"),
      Amount = decimal.Parse("500")
    };
    var discount = _mongoDbAccessor.GetPromotionDiscount("Student Promo");
    loanOrder.Promotions.Add(new Promotion("Student Promo", discount));

    _postgresDbAccessor.UpdatePromotionStatistics("Student Promo");
    return loanOrder;
  }
}