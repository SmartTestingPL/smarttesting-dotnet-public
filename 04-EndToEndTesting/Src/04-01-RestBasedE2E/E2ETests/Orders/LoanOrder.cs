using System;
using System.Collections.Generic;
using E2ETests.Customers;
using E2ETests.Lib;
using NodaTime;

namespace E2ETests.Orders;

/// <summary>
/// Reprezentuje wniosek o udzielenie pożyczki.
/// </summary>
public class LoanOrder
{
  public LoanOrder(Customer customer)
  {
    Customer = customer;
  }

  private void AddManagerDiscount(Guid managerId)
  {
    var promotion = new Promotion("Manager Promo: " + managerId, new decimal(50));
    Promotions.Add(promotion);
  }

  public Guid Guid { get; set; } = Guid.NewGuid();
  public Customer Customer { get; set; }
  public decimal Amount { get; set; }
  public decimal InterestRate { get; set; }
  public decimal Commission { get; set; }
  public List<Promotion> Promotions { get; } = new List<Promotion>();
  public LocalDate OrderDate { get; set; } = Clocks.ZonedUtc.GetCurrentDate();
  public OrderStatus Status { get; set; } = OrderStatus.New;

  public enum OrderStatus
  {
    New,
    Verified,
    Approved,
    Rejected
  }
}