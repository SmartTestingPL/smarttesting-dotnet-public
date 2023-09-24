using System;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

/// <summary>
/// Interfejs służący do komunikacji z relacyjną bazą danych.
/// Posłuży nam do przykładów zastosowania mocków i weryfikacji interakcji.
/// </summary>
public interface IPostgresAccessor
{
  void UpdatePromotionStatistics(string promotionName);

  void UpdatePromotionDiscount(string promotionName, decimal newDiscount);
}

/// <summary>
/// Interfejs służący do komunikacji z dokumentową bazą danych.
/// Posłuży nam do przykładów zastosowania stubów.
/// </summary>
public interface IMongoDbAccessor
{
  decimal GetPromotionDiscount(string promotionName);
}

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
        "Cannot order student loan if pl.smarttesting.customer is not a student.");
    }

    var now = Clocks.ZonedUtc.GetCurrentDate();
    var loanOrder = new LoanOrder(now, customer)
    {
      Type = LoanType.Student,
      Commission = 200
    };
    var discount = _mongoDbAccessor.GetPromotionDiscount("Student Promo");
    loanOrder.Promotions.Add(new Promotion("Student Promo", discount));

    _postgresDbAccessor.UpdatePromotionStatistics("Student Promo");
    return loanOrder;
  }
}