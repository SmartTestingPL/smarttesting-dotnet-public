using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BikService.Credit;

public interface ICreditInputQueue : IDisposable
{
  void Register(RabbitCreditInfoListener listener);
}

public class CreditInputRabbitMqQueue : ICreditInputQueue
{
  private const string QueueName = "creditInfo";
  private readonly ILogger<CreditInputRabbitMqQueue> _logger;
  private readonly IModel _channel;
  private readonly IConnection _connection;

  public CreditInputRabbitMqQueue(IOptions<RabbitMqOptions> options, ILogger<CreditInputRabbitMqQueue> logger)
  {
    _logger = logger;
    _logger.LogInformation("Connecting via " + options.Value.ConnectionString);
    _connection = new ConnectionFactory
    {
      Uri = new Uri(options.Value.ConnectionString)
    }.CreateConnection();
    _channel = _connection.CreateModel();
    _channel.QueueDeclare(QueueName, false, false, false, null);
  }

  public void Register(RabbitCreditInfoListener listener)
  {
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (model, ea) =>
    {
      try
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        listener.OnMessage(JsonConvert.DeserializeObject<CreditInfoDocument>(message)).Wait();
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Error while handling message from queue");
      }
    };
    _channel.BasicConsume(consumer, QueueName);
  }

  public void Dispose()
  {
    _channel.Dispose();
    _connection.Dispose();
  }
}