using System;
using System.Collections.Generic;
using NodaTime;
using ProductionCode.Customers;
using ProductionCode.Loans;

namespace ProductionCode.Orders;

public class LoanOrder
{
  public LoanOrder(LocalDate orderDate, Customer customer)
  {
    OrderDate = orderDate;
    Customer = customer;
  }

  private Customer Customer { get; set; }
  public LoanType Type { get; set; }
  public decimal Amount { set; get; }
  private decimal InterestRate { get; set; }
  public decimal Commission { get; set; }
  public List<Promotion> Promotions { get; } = new List<Promotion>();
  public LocalDate OrderDate { get; }

  public void AddManagerDiscount(Guid managerId)
  {
    var promotion = new Promotion($"Manager Promo: {managerId}", 50);
    Promotions.Add(promotion);
  }
}