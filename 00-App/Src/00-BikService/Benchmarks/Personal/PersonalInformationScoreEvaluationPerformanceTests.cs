using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Core.Scoring.domain;
using Core.Scoring.Personal;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Benchmarks.Personal;

// TODO: Dotyczy lekcji 05-05
[EventPipeProfiler(EventPipeProfile.CpuSampling)]
[SimpleJob(
  RuntimeMoniker.Net60,
  launchCount: 4,
  warmupCount: 1
)]
[IterationCount(10)]
[IterationTime(1000)]
public class PersonalInformationScoreEvaluationPerformanceTests
{
  private volatile IPersonalInformationClient _client = default!;
  private volatile IOccupationRepository _occupationRepository = default!;
  private volatile PersonalInformationScoreEvaluation _personalInformationScoreEvaluation = default!;

  [GlobalSetup]
  public void IterationSetup()
  {
    _client = Substitute.For<IPersonalInformationClient>();
    _occupationRepository = Substitute.For<IOccupationRepository>();
    _personalInformationScoreEvaluation = new PersonalInformationScoreEvaluation(
      _client,
      _occupationRepository,
      NullLogger<PersonalInformationScoreEvaluation>.Instance);

    _client.GetPersonalInformation(Arg.Any<Pesel>()).Returns(new PersonalInformation(PersonalInformation.Educations.Basic, 5, PersonalInformation.Occupations.Lawyer));
    _occupationRepository.GetOccupationScores().Returns(
      new Dictionary<PersonalInformation.Occupations?, Score>
      {
        [PersonalInformation.Occupations.Lawyer] = new(100)
      });
  }

  [Benchmark]
  public async Task Test()
  {
    (await _personalInformationScoreEvaluation.Evaluate(new Pesel("12345678901"))).Should().Be(new Score(130));
  }
}
