using System;
using System.Text;
using FraudDetection.Verifier;
using Core.Maybe;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FraudDetectionTests.Verifier;

public class FraudOutputQueue : IDisposable
{
  private readonly IModel _channel;
  private readonly IConnection _connection;
  private readonly string _queueName;

  public FraudOutputQueue(string connectionString)
  {
    _connection = new ConnectionFactory
    {
      Uri = new Uri(connectionString)
    }.CreateConnection();
    _channel = _connection.CreateModel();
    _queueName = "fraudOutput";
    _channel.QueueDeclare(_queueName, false, false, false, null);
  }

  public Maybe<CustomerVerification> Receive()
  {
    var result = _channel.BasicGet(_queueName, true);
    var body = result.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
      
    return JsonConvert.DeserializeObject<CustomerVerification>(message).Just();
  }

  public void Dispose()
  {
    _channel.Dispose();
    _connection.Dispose();
  }
}