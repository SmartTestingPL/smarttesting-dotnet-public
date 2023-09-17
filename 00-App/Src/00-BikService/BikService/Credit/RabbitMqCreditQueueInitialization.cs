namespace BikService.Credit;

public interface ICreditQueueInitialization
{
  void Perform(IServiceProvider appServices);
}

public class RabbitMqCreditQueueInitialization : ICreditQueueInitialization
{
  public void Perform(IServiceProvider appServices)
  {
    appServices.GetRequiredService<ICreditInputQueue>().Register(
      appServices.GetRequiredService<RabbitCreditInfoListener>());
  }
}