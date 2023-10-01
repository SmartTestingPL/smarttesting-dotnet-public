using System;

namespace ProductionCode.Loans;

public static class DecimalExtensions
{
  public static decimal Divide(this decimal d1, decimal d2, int decimals, MidpointRounding rounding)
  {
    var divide = decimal.Divide(d1, d2);
    return decimal.Round(divide, decimals, rounding);
  }

  public static decimal Divide(this decimal d1, decimal d2, MidpointRounding rounding)
  {
    var divide = decimal.Divide(d1, d2);
    return decimal.Round(divide, rounding);
  }

  public static decimal Add(this decimal d1, decimal d2)
  {
    return decimal.Add(d1, d2);
  }

  public static decimal Subtract(this decimal d1, decimal d2)
  {
    return decimal.Subtract(d1, d2);
  }

  public static decimal Multiply(this decimal d1, decimal d2)
  {
    return decimal.Multiply(d1, d2);
  }
}