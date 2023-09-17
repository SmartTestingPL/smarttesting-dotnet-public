using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FraudDetection.Verifier;

public interface IFraudDestination
{
  void Send<T>(string queueName, T item);
}

/// <summary>
/// W oryginale Javowym Spring udostępnia klasę RabbitTemplate.
/// Ta klasa jest jej odpowiednikiem w zakresie wysyłania wiadomości.
/// </summary>
public class RabbitMqDestination : IFraudDestination
{
  private readonly ConnectionFactory _connectionFactory;

  public RabbitMqDestination(IOptions<RabbitMqOptions> options)
  {
    _connectionFactory = new ConnectionFactory
    {
      Uri = new Uri(options.Value.ConnectionString)
    };
  }

  public void Send<T>(string queueName, T item)
  {
    using var connection = _connectionFactory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queueName, false, false, false, null);
    var json = JsonConvert.SerializeObject(item);
    channel.BasicPublish(
      string.Empty, 
      queueName, 
      null, 
      Encoding.UTF8.GetBytes(json));
  }
}