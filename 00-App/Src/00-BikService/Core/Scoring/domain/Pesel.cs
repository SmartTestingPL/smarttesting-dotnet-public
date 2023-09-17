namespace Core.Scoring.domain;

public class Pesel : IEquatable<Pesel>
{
  public Pesel(string value)
  {
    if (value.Length != 11)
    {
      throw new ArgumentException("PESEL must be of 11 chars");
    }
    Value = value;
  }

  public string Value { get; private set; }

  public string Obfuscated()
  {
    return Value.Substring(7);
  }

  public override string ToString()
  {
    return Obfuscated();
  }

  public bool Equals(Pesel? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return Value == other.Value;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != GetType()) return false;
    return Equals((Pesel)obj);
  }

  public override int GetHashCode()
  {
    return Value.GetHashCode();
  }

  public static bool operator ==(Pesel? left, Pesel? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(Pesel? left, Pesel? right)
  {
    return !Equals(left, right);
  }
}
