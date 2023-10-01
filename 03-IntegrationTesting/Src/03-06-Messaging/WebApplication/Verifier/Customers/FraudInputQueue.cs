using System;
using System.Text;
using Core.NullableReferenceTypesExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApplication.Verifier.Customers;

/// <summary>
/// W wersji Javowej połączenie z RabbitMq
/// dzieje się przy pomocy klas Springa.
/// Dla Asp.Net Core nie znalazłem takiej integracji,
/// więc musiałem zaimplementować taki prymitywny mechanizm samemu.
/// Uwaga: to nie jest reprezentatywny przykład, jak produkcyjnie
/// łączyć się z RabbitMq.
/// </summary>
public class FraudInputQueue : IDisposable
{
  private readonly ILogger<FraudInputQueue> _logger;
  private readonly IModel _channel;
  private readonly IConnection _connection;

  public FraudInputQueue(IOptions<RabbitMqConfiguration> options, ILogger<FraudInputQueue> logger)
  {
    _logger = logger;
    _connection = new ConnectionFactory
    {
      Uri = new Uri(options.Value.ConnectionString)
    }.CreateConnection();
    _channel = _connection.CreateModel();
    _channel.QueueDeclare("fraudInput", false, false, false, null);
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
        // rejestrujemy obserwatora kolejki
        listener.OnFraud(JsonConvert.DeserializeObject<CustomerVerification>(message).OrThrow()).Wait();
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