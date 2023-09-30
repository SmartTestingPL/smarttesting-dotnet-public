using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApplication.Verifier;

public interface IFraudInputQueue : IDisposable
{
  void Register(IFraudListener listener);
}

/// <summary>
/// W wersji Javowej połączenie z RabbitMq
/// dzieje się przy pomocy klas Springa.
/// Dla Asp.Net Core nie znalazłem takiej integracji,
/// więc musiałem zaimplementować taki prymitywny mechanizm samemu.
/// Uwaga: to nie jest reprezentatywny przykład, jak produkcyjnie
/// łączyć się z RabbitMq.
/// </summary>
public class FraudInputQueue : IFraudInputQueue
{
  private readonly ILogger<FraudInputQueue> _logger;
  private IModel _channel;
  private IConnection _connection;

  public FraudInputQueue(IOptions<RabbitMqConfiguration> options, ILogger<FraudInputQueue> logger)
  {
    _logger = logger;
    Policy
      .Handle<Exception>()
      .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(3)
      ).Execute(() =>
      {
        var connectionFactory = new ConnectionFactory
        {
          Uri = new Uri(options.Value.ConnectionString)
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("fraudInput", false, false, false, null);
      });
  }

  public void Register(IFraudListener listener)
  {
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (model, ea) =>
    {
      try
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        listener.OnFraud(JsonConvert.DeserializeObject<CustomerVerification>(message)).Wait();
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Error while handling message from queue");
      }
    };
    _channel.BasicConsume(consumer, "fraudInput");
  }

  public void Dispose()
  {
    _channel.Dispose();
    _connection.Dispose();
  }
}