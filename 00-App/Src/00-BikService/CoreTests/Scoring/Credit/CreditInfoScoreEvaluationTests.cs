using System.Threading.Tasks;
using Core.Scoring.Credit;
using Core.Scoring.domain;
using CoreTests.Scoring.Cost;
using CoreTests.Scoring.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using static Core.Scoring.Credit.CreditInfo.DebtPaymentHistoryStatus;

namespace CoreTests.Scoring.Credit;

public class CreditInfoScoreEvaluationTests
{
  private ICreditInfoRepository _repository = default!;
  private CreditInfoScoreEvaluation _scoreEvaluation = default!;

  [SetUp]
  public void SetUp()
  {
    _repository = Substitute.For<ICreditInfoRepository>();
    _scoreEvaluation = new CreditInfoScoreEvaluation(_repository, NullLogger<CreditInfoScoreEvaluation>.Instance);
  }

  [Test]
  public async Task ShouldReturnZeroForNullCreditInfo()
  {
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(null as CreditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullCreditInfoFieldsPresent()
  {
    var creditInfo = new CreditInfo(null, null, null);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi; przedział powyżej 10000 nie został zaimplementowany;
  // obsługa niepoprawnej wartości -1 nie została zaimplementowana
  [Ignore("")]
  [Description("The score for livingCost equal {0} should be {1}.")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/living-cost.csv", typeof(LivingCostRecord) })]
  public async Task ShouldEvaluateScoreBasedOnCurrentLivingCost(LivingCostRecord record)
  {
    var creditInfo = new CreditInfo(decimal.Parse("5501"), decimal.Parse(record.LivingCost),
      NotASinglePaidInstallment);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  // Test nie przechodzi; przedział powyżej 10000 nie został zaimplementowany;
  // obsługa niepoprawnej wartości -1 nie została zaimplementowana
  [Ignore("")]
  [Description("The score for currentDebt equal {0} should be {1}.")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/current-debt.csv", typeof(CurrentDebtRecord) })]
  public async Task ShouldEvaluateScoreBasedOnCurrentDebt(CurrentDebtRecord record)
  {
    var creditInfo = new CreditInfo(decimal.Parse(record.CurrentDebt), decimal.Parse("6501"),
      NotASinglePaidInstallment);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  [Test]
  public async Task ShouldEvaluateScoreForNotPayingCustomer()
  {
    var creditInfo = new CreditInfo(decimal.Parse("5501"), decimal.Parse("6501"),
      NotASinglePaidInstallment);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(0);
  }

  [Test]
  public async Task ShouldEvaluateScoreForAlwaysPayingCustomer()
  {
    var creditInfo = new CreditInfo(decimal.Parse("5501"), decimal.Parse("6501"),
      NotASingleUnpaidInstallment);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(50);
  }

  [Test]
  public async Task ShouldEvaluateScoreForOftenMissingPaymentCustomer()
  {
    var creditInfo = new CreditInfo(decimal.Parse("5501"), decimal.Parse("6501"),
      MultipleUnpaidInstallments);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(10);
  }

  [Test]
  public async Task ShouldEvaluateScoreForRarelyMissingPaymentCustomer()
  {
    var creditInfo = new CreditInfo(decimal.Parse("5501"), decimal.Parse("6501"),
      IndividualUnpaidInstallments);
    _repository.FindCreditInfo(Arg.Any<Pesel>()).Returns(creditInfo);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(30);
  }

}

public record CurrentDebtRecord(string CurrentDebt, int Points);
public record LivingCostRecord(string LivingCost, int Points);
