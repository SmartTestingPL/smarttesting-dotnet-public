﻿namespace LoanOrders.Orders;

public class Promotion
{
  public Promotion(string name, decimal discount)
  {
    Name = name;
    Discount = discount;
  }

  public string Name { get; }
  public decimal Discount { get; }
}