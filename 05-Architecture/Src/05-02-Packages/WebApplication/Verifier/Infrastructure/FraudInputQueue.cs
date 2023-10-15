using System;
using System.Text;
using Core.Verifier;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebApplication.Verifier.Model;

namespace WebApplication.Verifier.Infrastructure;

public class FraudInputQueue : IFraudInputQueue
{
  private readonly ILogger<FraudInputQueue> _logger;
  private IModel? _channel = default!;
  private IConnection? _connection = default!;

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