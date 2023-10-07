namespace FraudVerifier;

public class VerificationEvent
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
}