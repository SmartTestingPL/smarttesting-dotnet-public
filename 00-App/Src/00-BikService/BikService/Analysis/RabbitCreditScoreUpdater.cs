using System.Text;
using BikService.Credit;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BikService.Analysis;

public class RabbitCreditScoreUpdater : IScoreUpdater
{
  private const string QueueName = "scoreQueue";
  private readonly ILogger<RabbitCreditScoreUpdater> _log;
  private readonly ConnectionFactory _connectionFactory;

  public RabbitCreditScoreUpdater(IOptions<RabbitMqOptions> options, ILogger<RabbitCreditScoreUpdater> log)
  {
    _connectionFactory = new ConnectionFactory
    {
      Uri = new Uri(options.Value.ConnectionString)
    };
    _log = log;
  }

  public void ScoreCalculated(ScoreCalculatedEvent scoreCalculatedEvent)
  {
    _log.LogInformation($"Sending out the Score calculated event {scoreCalculatedEvent}");
    using var connection = _connectionFactory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue: QueueName, false, false, false, null);
    var json = JsonConvert.SerializeObject(scoreCalculatedEvent);
    channel.BasicPublish(string.Empty, QueueName, null, Encoding.UTF8.GetBytes(json));
  }
}