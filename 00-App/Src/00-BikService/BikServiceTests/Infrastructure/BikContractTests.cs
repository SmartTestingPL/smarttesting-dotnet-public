using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtmaFileSystem;
using BikService.Credit;
using BikService.Personal;
using Core.Scoring.Analysis;
using Core.Scoring.domain;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;

namespace BikServiceTests.Infrastructure;

// TODO: Dotyczy lekcji 05-03. Jest odpowiednikiem Javowego BikContractBaseClass
public class BikContractTests
{
  private KestrelBikWebApp _app = default!;

  [SetUp]
  public void SetUp()
  {
    _app = new KestrelBikWebApp(collection =>
    {
      collection.AddSingleton(MockedScoreAnalyzer());
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

  private static IScoreAnalyzer MockedScoreAnalyzer()
  {
    var verifier = Substitute.For<IScoreAnalyzer>();
    verifier.ShouldGrantLoan(new Pesel("89050193724")).Returns(true);
    return verifier;
  }

  [Test]
  public void ShouldHonourPactWithConsumer()
  {
    var pactVerifier = new PactVerifier(
      new PactVerifierConfig {
        Outputters = new List<IOutput> {
          new ConsoleOutput()
        }
      });
    pactVerifier
      .ServiceProvider("BikService", new Uri(_app.ServerAddress))
      .WithFileSource(
        AbsoluteFilePath.OfThisFile()
          .ParentDirectory(3).Value()
          .AddDirectoryName("Contracts")
          .AddDirectoryName("Http")
          .AddFileName("FraudDetection-BikService.json").Info())
      .Verify();
  }
}
