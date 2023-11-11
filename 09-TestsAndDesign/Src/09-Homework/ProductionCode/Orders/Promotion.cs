using System;

namespace ProductionCode.Orders;

public class Promotion : IEquatable<Promotion>
{
  public Promotion(string name, decimal discount)
  {
    Name = name;
    Discount = discount;
  }

  public string Name { get; }
  public decimal Discount { get; }

  public bool Equals(Promotion? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return Name == other.Name && Discount == other.Discount;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((Promotion)obj);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Name, Discount);
  }

  public static bool operator ==(Promotion? left, Promotion? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(Promotion? left, Promotion? right)
  {
    return !Equals(left, right);
  }
}