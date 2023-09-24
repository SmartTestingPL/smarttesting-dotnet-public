namespace ProductionCode.Orders;

/// <summary>
/// Reprezentuje promocję dla oferty pożyczek.
/// </summary>
public class Promotion
{
  internal Promotion(string name, decimal discount)
  {
    Name = name;
    Discount = discount;
  }

  public string Name { get; }
  public decimal Discount { get; }
}