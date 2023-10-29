using System;
using System.Collections.Generic;
using LoanOrders.Customers;
using LoanOrders.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime;

namespace LoanOrders.Orders;

/// <summary>
/// Reprezentuje wniosek o udzielenie pożyczki.
/// </summary>
public partial class LoanOrder
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

  public Customer Customer { get; }
  public decimal Amount { get; set; }
  public decimal InterestRate { get; set; }
  public decimal Commission { get; set; }
  public List<Promotion> Promotions { get; } = new List<Promotion>();
  public LocalDate OrderDate { get; } = Clocks.ZonedUtc.GetCurrentDate();

  [JsonConverter(typeof(StringEnumConverter))]
  public CustomerOrderStatus Status { set; get; } = CustomerOrderStatus.New;

  public Guid Guid { get; set; } = Guid.NewGuid();
}