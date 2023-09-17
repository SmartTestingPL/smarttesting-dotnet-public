using System.Collections.Generic;
using System.Threading.Tasks;
using AtmaFileSystem;
using BikService;
using BikService.Credit;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PactNet.Verifier;
using Testcontainers.RabbitMq;

namespace BikServiceTests.Analysis;

// Dotyczy lekcji 05-03
public class AnalysisMessagingContractTests
{
  private RabbitMqContainer _rabbitMqTestContainer = default!;
  private ScoreOutputQueue _outputQueue = default!;
  private IScoreUpdater _scoreUpdater = default!;
  private ServiceProvider _serviceProvider = default!;

  [SetUp]
  public async Task InitializeContainer()
  {
    _rabbitMqTestContainer =
      new RabbitMqBuilder()
        .WithCleanUp(true)
        .WithImage("rabbitmq:3.7.25-management-alpine")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _rabbitMqTestContainer.StartAsync();

    _outputQueue = new ScoreOutputQueue(_rabbitMqTestContainer.GetConnectionString());
    _serviceProvider = GetServiceProvider(_rabbitMqTestContainer.GetConnectionString());
    _scoreUpdater = _serviceProvider.GetRequiredService<IScoreUpdater>();
  }

  [TearDown]
  public async Task TearDown()
  {
    _outputQueue.Dispose();
    await _serviceProvider.DisposeAsync();
    await _rabbitMqTestContainer.DisposeAsync();
  }

  [Test]
  public void ShouldHonourPactWithConsumer()
  {
    _scoreUpdater.ScoreCalculated(new ScoreCalculatedEvent(new Pesel("12345678901"), new Score(100)));

    using var verifier = new PactVerifier();
    verifier
      .MessagingProvider("FraudDetection")
      .WithProviderMessages(scenarios =>
      {
        scenarios.Add("should produce a score calculation event", () => new List<ScoreCalculatedEvent> 
        {
          _outputQueue.Receive().Value()
        });
      })
      .WithFileSource(AbsoluteFilePath.OfThisFile()
        .ParentDirectory(3).Value()
        .AddDirectoryName("Contracts")
        .AddDirectoryName("Messaging")
        .AddFileName("ScoreUpdateEventConsumer-BikService.json").Info())
      .Verify();
  }

  private static ServiceProvider GetServiceProvider(string rabbitConnectionString)
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddAppServices();
    serviceCollection.AddSingleton(_ => Options.Create(new RabbitMqOptions 
    {
      ConnectionString = rabbitConnectionString
    }));
    var serviceProvider = serviceCollection.BuildServiceProvider();
    return serviceProvider;
  }
}