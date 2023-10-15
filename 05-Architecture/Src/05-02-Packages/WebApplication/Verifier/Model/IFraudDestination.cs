namespace WebApplication.Verifier.Model;

public interface IFraudDestination
{
  void Send<T>(string queueName, T item);
}