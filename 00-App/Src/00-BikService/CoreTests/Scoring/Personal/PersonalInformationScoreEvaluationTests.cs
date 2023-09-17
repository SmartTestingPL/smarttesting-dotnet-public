using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Scoring.domain;
using Core.Scoring.Personal;
using CoreTests.Scoring.Cost;
using CoreTests.Scoring.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Scoring.Personal;

public class PersonalInformationScoreEvaluationTests
{
  private IPersonalInformationClient _client = default!;
  private PersonalInformationScoreEvaluation _scoreEvaluation = default!;

  [SetUp]
  public void SetUp()
  {
    _client = Substitute.For<IPersonalInformationClient>();
    _scoreEvaluation = new PersonalInformationScoreEvaluation(
      _client,
      new TestOccupationRepository(),
      NullLogger<PersonalInformationScoreEvaluation>.Instance);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullPersonalInformation()
  {
    _client.GetPersonalInformation(Arg.Any<Pesel>()).Returns(null as PersonalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi; obsługa nulli nie została zaimplementowana
  [Ignore("")]
  [Test]
  public async Task ShouldReturnZeroWhenNullValues()
  {
    var personalInformation = new PersonalInformation(null, 0, null);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
      .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.Should().Be(Score.Zero);
  }

  // Test nie przechodzi - brakuje implementacji dla > 30 lat doświadczenia
  [Ignore("")]
  [Description("The score for {0} number of dependants should be equal to {1}")]
  [TestCaseSource(typeof(DataProviders), nameof(DataProviders.CsvDataProvider), new object[] { "Resources/work-experience.csv", typeof(WorkExperienceRecord) })]
  public async Task ShouldCalculateScoreBasedOnYearsOfExperience(WorkExperienceRecord record)
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.None, record.YearsOfWorkExperience, PersonalInformation.Occupations.Other);
    _client.GetPersonalInformation(Arg.Any<Pesel>()).Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(record.Points);
  }

  [Test]
  public async Task ShouldCalculateScoreWhenForOccupationPresentInRepository()
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.None, 0, PersonalInformation.Occupations.Programmer);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
        .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(30);
  }

  [Test]
  public async Task ShouldUseZeroPointsDefaultWhenForOccupationNotInRepositoryAndNoEducation()
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.None, 0, PersonalInformation.Occupations.Doctor);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
        .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(0);
  }

  [Test]
  public async Task ShouldCalculateScoreForBasicEducation()
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.Basic, 0, PersonalInformation.Occupations.Doctor);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
        .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(10);
  }

  [Test]
  public async Task ShouldCalculateScoreForMediumEducation()
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.Medium, 0, PersonalInformation.Occupations.Doctor);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
        .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(30);
  }

  [Test]
  public async Task ShouldCalculateScoreForHighEducation()
  {
    var personalInformation = new PersonalInformation(PersonalInformation.Educations.High, 0, PersonalInformation.Occupations.Doctor);
    _client.GetPersonalInformation(Arg.Any<Pesel>())
        .Returns(personalInformation);

    var score = await _scoreEvaluation.Evaluate(TestUtils.AnId());

    score.GetPoints().Should().Be(50);
  }
}

public record WorkExperienceRecord(int YearsOfWorkExperience, int Points);

// Test double dla OccupationRepository; dodaliśmy tylko jeden element do mapy, bo na potrzeby tych testów,
// interesują nas tak na prawdę tutaj tylko 2 sytuacje: 1) dany zawód jest w repozytorium, 2) danego zawodu nie ma w repozytorium
class TestOccupationRepository : IOccupationRepository
{

  public Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    return new Dictionary<PersonalInformation.Occupations?, Score>
    {
      [PersonalInformation.Occupations.Programmer] = new(30)
    };
  }
}
