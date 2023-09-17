using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BikServiceTests.Credit;

public class RabbitMqDestination
{
  private readonly ConnectionFactory _connectionFactory;

  public RabbitMqDestination(string connectionString)
  {
    _connectionFactory = new ConnectionFactory
    {
      Uri = new Uri(connectionString)
    };
  }

  public void Send<T>(string queueName, T item)
  {
    using var connection = _connectionFactory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue: queueName, false, false, false, null);
    var json = JsonConvert.SerializeObject(item);
    channel.BasicPublish(string.Empty, queueName, null, Encoding.UTF8.GetBytes(json));
  }
}