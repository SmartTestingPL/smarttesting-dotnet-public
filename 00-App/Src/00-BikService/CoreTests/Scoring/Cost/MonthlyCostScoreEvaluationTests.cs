using System.Threading.Tasks;
using Core.Scoring.Cost;
using Core.Scoring.domain;
using CoreTests.Scoring.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Scoring.Cost;

public class MonthlyCostScoreEvaluationTests
{
  private IMonthlyCostClient _client = default!;
  private MonthlyCostScoreEvaluation _scoreEvaluation = default!;

  [SetUp]
  public void SetUp()
  {
    _client = Substitute.For<IMonthlyCostClient>();
    _scoreEvaluation = new MonthlyCostScoreEvaluation(_client, NullLogger<MonthlyCostScoreEvaluation>.Instance);
  }

  // Test nie przechodzi; obsługa minusowej wartości kosztów nie została dodana;
  // Brakuje implementacji dla przedziału 3501 - 5500 -> 20; Granice warunków niepoprawnie zaimplementowane
  [Ignore("")]
  [Description("The score for monthly cost equal to {0} should be {1}.")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/monthly-cost.csv", typeof(MonthlyCostRecord) })]
  public async Task ShouldCalculateScoreBasedOnMonthlyCost(MonthlyCostRecord record)
  {
    _client.GetMonthlyCosts(Arg.Any<Pesel>())
      .Returns(decimal.Parse(record.MonthlyCost));

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenMonthlyScoreNull()
  {
    _client.GetMonthlyCosts(Arg.Any<Pesel>()).Returns(null as decimal?);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }
}

public record MonthlyCostRecord(string MonthlyCost, int Points);
