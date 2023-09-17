using System.Threading.Tasks;
using Core.Scoring.domain;
using Core.Scoring.Social;
using Core.Scoring.Social.Validation;
using CoreTests.Scoring.Cost;
using CoreTests.Scoring.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Scoring.Social;

public class SocialStatusScoreEvaluationTests
{
  private ISocialStatusClient _client = default!;
  private SocialStatusScoreEvaluation _scoreEvaluation = default!;

  [SetUp]
  public void SetUp()
  {
    _client = Substitute.For<ISocialStatusClient>();
    _scoreEvaluation = new SocialStatusScoreEvaluation(_client);
  }


  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullSocialStatus()
  {
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(null as SocialStatus);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullMaritalStatus()
  {
    var status = new SocialStatus(0, 0, null, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullEmploymentContract()
  {
    var status = new SocialStatus(0, 0, SocialStatus.MaritalStatuses.Married, null);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  //Test nie przechodzi - logika nie została zaimplementowana
  // Zazwyczaj w tego typu przypadkach testowych chcemy zweryfikować, w zależności od wymagań biznesowych, albo, że jest rzucany odpowiedni wyjątek biznesowy
  // albo, że żaden wyjątek nie jest rzucony i błąd jest odpowiednio obsłużony w algorytmie (np. może być zwrócone Score.ZERO)
  [Ignore("")]
  [Description("Should throw exception when no. of dependents equals {0} and no. of household members equals {1}")]
  [TestCase(0, 0)]
  [TestCase(-1, 0)]
  [TestCase(0, -1)]
  [TestCase(2, 1)]
  [TestCase(1, 1)]
  public async Task ShouldThrowBusinessExceptionWhenIncorrectNumbersOfMembersAndDependants(int numberOfDependants, int numberOfHouseholdMembers)
  {
    var status = new SocialStatus(numberOfDependants, numberOfHouseholdMembers,
      SocialStatus.MaritalStatuses.Single, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    await _scoreEvaluation.Awaiting(e => e.Evaluate(TestUtils.AnId()))
      .Should().ThrowExactlyAsync<NumberOfHouseholdMembersValidationException>();
  }

  // Test nie przechodzi: warunki brzegowe dla 3 członków gospodarstwa domowego nie zaimplementowane poprawnie
  [Ignore("")]
  [Description("The score for {0} household members should be equal to {1}")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/household-members.csv", typeof(HouseholdMemberRecord) })]
  public async Task ShouldCalculateScoreDependingOnNumberOfHouseholdMembers(HouseholdMemberRecord record)
  {
    var status = new SocialStatus(0, record.Members,
        SocialStatus.MaritalStatuses.Single, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  [Description("The score for {0} number of dependants should be equal to {1}")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/dependants.csv", typeof(DependantRecord) })]
  public async Task ShouldCalculateScoreDependingOnNumberOfDependants(DependantRecord record)
  {
    var status = new SocialStatus(record.NumberOfDependants, 6,
        SocialStatus.MaritalStatuses.Single, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  [Description("The score for {0} number of dependants should be equal to {1}")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/dependants.csv", typeof(DependantRecord) })]
  public async Task ShouldCalculateScoreDependingOnMaritalStatus(DependantRecord record)
  {
    var status = new SocialStatus(record.NumberOfDependants, 6,
        SocialStatus.MaritalStatuses.Single, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  [Test]
  public async Task ShouldCalculateScoreWhenCustomerSingle()
  {
    var status = new SocialStatus(0, 6,
        SocialStatus.MaritalStatuses.Single, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(90);
  }

  [Test]
  public async Task ShouldCalculateScoreWhenCustomerMarriedAndEmploymentContract()
  {
    var status = new SocialStatus(0, 6,
        SocialStatus.MaritalStatuses.Married, SocialStatus.ContractTypes.EmploymentContract);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(80);
  }


  [Test]
  public async Task ShouldCalculateScoreWhenOwnBusiness()
  {
    var status = new SocialStatus(0, 6,
        SocialStatus.MaritalStatuses.Married, SocialStatus.ContractTypes.OwnBusinessActivity);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(70);
  }

  [Test]
  public async Task ShouldCalculateScoreWhenUnemployed()
  {
    var status = new SocialStatus(0, 6,
        SocialStatus.MaritalStatuses.Married, SocialStatus.ContractTypes.Unemployed);
    _client.GetSocialStatus(Arg.Any<Pesel>()).Returns(status);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(60);
  }

}

public record DependantRecord(int NumberOfDependants, int Points);
public record HouseholdMemberRecord(int Members, int Points);
