using BenchmarkDotNet.Running;
using Benchmarks.Personal;
using FluentAssertions;

var summary = BenchmarkRunner.Run<PersonalInformationScoreEvaluationPerformanceTests>();
summary.ValidationErrors.Should().BeEmpty();
summary.HasCriticalValidationErrors.Should().BeFalse();