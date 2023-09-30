using System;
using System.Collections.Generic;
using NodaTime;
using ProductionCode.Customers;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

/// <summary>
/// Reprezentuje wniosek o udzielenie pożyczki.
/// </summary>
public class LoanOrder
{
  public LoanOrder(
    LocalDate orderDate, 
    Customer customer, 
    decimal? amount, 
    decimal? interestRate,
    decimal? commission)
  {
    OrderDate = orderDate;
    Customer = customer;
    Amount = amount;
    InterestRate = interestRate;
    Commission = commission;
  }

  private Customer Customer { get; set; }
  public LoanType Type { get; set; }
  public decimal? Amount { set; get; }
  public decimal? InterestRate { get; set; }
  public decimal? Commission { get; set; }
  public List<Promotion> Promotions { get; } = new();
  public LocalDate OrderDate { get; }

  public void AddManagerDiscount(Guid managerId)
  {
    var promotion = new Promotion($"Manager Promo: {managerId}", 50);
    Promotions.Add(promotion);
  }
}