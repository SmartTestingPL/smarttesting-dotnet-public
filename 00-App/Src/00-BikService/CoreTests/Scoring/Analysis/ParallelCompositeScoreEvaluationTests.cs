using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Scoring;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using Extensions.Logging.NUnit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Scoring.Analysis;

// TODO: Dotyczy lekcji 08-02
public class ParallelCompositeScoreEvaluationTests
{
  private IScoreUpdater _scoreUpdater = default!;
  private ILogger _logger = default!;
  private ILogger<ParallelCompositeScoreEvaluation> _productionLogger = default!;

  [SetUp]
  public void SetUp()
  {
    _scoreUpdater = Substitute.For<IScoreUpdater>();
    _logger = new NUnitLogger(nameof(ParallelCompositeScoreEvaluationTests));
    _productionLogger = new LoggerFactory(new[] { new NUnitLoggerProvider() })
      .CreateLogger<ParallelCompositeScoreEvaluation>();
  }

  [Test]
  public async Task ShouldCalculateScores()
  {
    var evaluation = new ParallelCompositeScoreEvaluation(
      new List<IScoreEvaluation>
      {
        new TenScoreEvaluation(_logger),
        new TwentyScoreEvaluation(_logger)
      },
      _scoreUpdater,
      Executors.TaskBased(),
      _productionLogger);

    var score = await evaluation.AggregateAllScores(new Pesel("12345678901"));

    score.Points.Should().Be(30);
    _scoreUpdater.Received(1).ScoreCalculated(Arg.Is<ScoreCalculatedEvent>(@event =>
      @event.Score.Points == 30 && @event.Pesel.Value.Equals("12345678901")));
  }

  [Test]
  [Ignore("Test wykryje błąd")]
  public async Task ShouldReturn0ScoreWhenExceptionThrown()
  {
    var evaluation = new ParallelCompositeScoreEvaluation(
      new List<IScoreEvaluation> { new ExceptionScoreEvaluation(_productionLogger) },
      _scoreUpdater,
      Executors.NewSingleThreadExecutor(),
      _productionLogger);

    var score = await evaluation.AggregateAllScores(new Pesel("12345678901"));

    score.Should().Be(Score.Zero);
    _scoreUpdater.Received(1).ScoreCalculated(Arg.Is<ScoreCalculatedEvent>(@event => Score.Zero.Equals(@event.Score) && @event.Pesel.Value.Equals("12345678901")));
  }

  class TenScoreEvaluation : IScoreEvaluation
  {
    private readonly ILogger _log;

    public TenScoreEvaluation(ILogger log)
    {
      _log = log;
    }

    public async Task<Score> Evaluate(Pesel pesel)
    {
      _log.LogInformation("Hello from 10");
      return await Task.FromResult(new Score(10));
    }
  }

  class TwentyScoreEvaluation : IScoreEvaluation
  {
    private readonly ILogger _log;

    public TwentyScoreEvaluation(ILogger log)
    {
      _log = log;
    }

    public async Task<Score> Evaluate(Pesel pesel)
    {
      _log.LogInformation("Hello from 20");
      return await Task.FromResult(new Score(20));
    }
  }

  class ExceptionScoreEvaluation : IScoreEvaluation
  {
    private readonly ILogger _log;

    public ExceptionScoreEvaluation(ILogger log)
    {
      _log = log;
    }

    public Task<Score> Evaluate(Pesel pesel)
    {
      _log.LogInformation("Hello from exception");
      throw new InvalidOperationException("Boom!");
    }
  }
}

internal class Executors
{
  public static IExecutor TaskBased()
  {
    return new TaskBasedExecutor();
  }

  public static IExecutor NewSingleThreadExecutor()
  {
    return new SynchronousExecutor();
  }
}

internal class SynchronousExecutor : IExecutor
{
  public Task<TResult> Run<TResult>(Func<Task<TResult>> func)
  {
    return Task.FromResult(func().Result);
  }
}
