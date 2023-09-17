using FraudDetection;

namespace FraudDetectionTests.Verifier;

public class SimpleObjectProvider<T> : IObjectProvider<T>
{
  private readonly T _value;

  public SimpleObjectProvider(T value)
  {
    _value = value;
  }

  public T? GetIfAvailable()
  {
    return _value;
  }
}