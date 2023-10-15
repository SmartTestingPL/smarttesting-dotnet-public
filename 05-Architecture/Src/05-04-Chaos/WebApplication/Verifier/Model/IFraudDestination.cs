namespace WebApplication.Verifier.Model;

/// <summary>
/// Interfejs dla brokera kolejek
/// </summary>
public interface IFraudDestination
{
  void Send<T>(string queueName, T item);
}