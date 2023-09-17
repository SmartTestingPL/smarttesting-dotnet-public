using System;
using System.Text;
using Core.Maybe;
using Core.Scoring.domain;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BikServiceTests.Analysis;

public class ScoreOutputQueue : IDisposable
{
  private readonly IModel _channel;
  private readonly IConnection _connection;
  private readonly string _queueName;

  public ScoreOutputQueue(string connectionString)
  {
    _connection = new ConnectionFactory
    {
      Uri = new Uri(connectionString)
    }.CreateConnection();
    _channel = _connection.CreateModel();
    _queueName = "scoreQueue";
    _channel.QueueDeclare(_queueName, false, false, false, null);
  }

  public Maybe<ScoreCalculatedEvent> Receive()
  {
    var result = _channel.BasicGet(_queueName, true);
    var body = result.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
      
    return JsonConvert.DeserializeObject<ScoreCalculatedEvent>(message).Just();
  }

  public void Dispose()
  {
    _channel.Dispose();
    _connection.Dispose();
  }
}