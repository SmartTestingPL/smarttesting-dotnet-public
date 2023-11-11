using System;

namespace ProductionCode.Verifier;

public class VerificationEvent : IEquatable<VerificationEvent>
{
  private readonly bool _passed;

  public VerificationEvent(bool passed)
  {
    _passed = passed;
  }

  public bool Passed()
  {
    return _passed;
  }

  public bool Equals(VerificationEvent? other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return _passed == other._passed;
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((VerificationEvent)obj);
  }

  public override int GetHashCode()
  {
    return _passed.GetHashCode();
  }

  public static bool operator ==(VerificationEvent? left, VerificationEvent? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(VerificationEvent? left, VerificationEvent? right)
  {
    return !Equals(left, right);
  }
}