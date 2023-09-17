using System.Net;
using System.Threading.Tasks;
using BikService.Credit;
using BikService.Personal;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using FluentAssertions;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace BikServiceTests.Infrastructure;

// TODO: Dotyczy lekcji 03-03
public class BikControllerTests
{
  private BikWebApp _app = default!;
  private IScoreAnalyzer _scoreAnalyzer = default!;

  [SetUp]
  public void SetUp()
  {
    _scoreAnalyzer = Substitute.For<IScoreAnalyzer>();
    _scoreAnalyzer.ShouldGrantLoan(new Pesel("89050193724")).Returns(true);
    _scoreAnalyzer.ShouldGrantLoan(new Pesel("00262161334")).Returns(false);
    _app = new BikWebApp(collection =>
    {
      collection.AddSingleton(_scoreAnalyzer);
      collection.AddSingleton(Substitute.For<IMongoDbInitialization>());
      collection.AddSingleton(Substitute.For<IOccupationRepositoryInitialization>());
      collection.AddSingleton(Substitute.For<ICreditQueueInitialization>());
    });
  }

  [TearDown]
  public async Task TearDown()
  {
    await _app.DisposeAsync();
  }

  [Test]
  public async Task ShouldReturnStatusVerificationPassedForNonFraud()
  {
    var response = await _app.Request("/89050193724").GetAsync();
    response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    (await response.GetStringAsync()).Should().Contain("status\":\"VerificationPassed\"");
  }

  [Test]
  public async Task ShouldReturnStatusVerificationFailedForFraud()
  {
    var response = await _app.Request("/00262161334").AllowAnyHttpStatus().GetAsync();
    response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
    (await response.GetStringAsync()).Should().Contain("status\":\"VerificationFailed\"");
  }
}

