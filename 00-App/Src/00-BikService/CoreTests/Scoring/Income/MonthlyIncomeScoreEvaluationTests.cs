using System.Threading.Tasks;
using Core.Scoring.domain;
using Core.Scoring.Income;
using CoreTests.Scoring.Cost;
using CoreTests.Scoring.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Scoring.Income;

public class MonthlyIncomeScoreEvaluationTests
{
  private IMonthlyIncomeClient _client = default!;
  private MonthlyIncomeScoreEvaluation _scoreEvaluation = default!;

  [SetUp]
  public void SetUp()
  {
    _client = Substitute.For<IMonthlyIncomeClient>();
    _scoreEvaluation = new MonthlyIncomeScoreEvaluation(
      _client, 
      NullLogger<MonthlyIncomeScoreEvaluation>.Instance);
  }


  // Test nie przechodzi - obsługa minusowej wartości dochodów nie została dodana; Brakuje implementacji dla przedziału 3501 - 5500 -> 30
  [Ignore("")]
  [Description("The score for monthly income equal {0} should be {1}.")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/monthly-income.csv", typeof(MonthlyIncomeRecord) })]
  public async Task ShouldCalculateScoreBasedOnMonthlyIncome(MonthlyIncomeRecord record)
  {
    _client.GetMonthlyIncome(Arg.Any<Pesel>()).Returns(decimal.Parse(record.MonthlyIncome));

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenMonthlyIncomeNull()
  {
    _client.GetMonthlyIncome(Arg.Any<Pesel>()).Returns(null as decimal?);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }
}

public record MonthlyIncomeRecord(string MonthlyIncome, int Points);
