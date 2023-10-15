using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Core.Customers;
using Core.Lib;
using Core.Verifier;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Benchmarks;

/// <summary>
/// Microbenchmark dla <see cref="CustomerVerifier"/>
/// </summary>
[EventPipeProfiler(EventPipeProfile.CpuSampling)] //Sposób pomiaru
[SimpleJob(
  RuntimeMoniker.Net70,
  launchCount: 1, //jedno odpalenie pomiaru
  warmupCount: 2  //dwie iteracje rozgrzewające maszynę wirtualną
)]
[IterationCount(2)] //dwie iteracje pomiarowe
[IterationTime(10000)] //czas pojedynczej iteracji: 10 sekund
public class CustomerVerifierBenchmarks
{
  private Customer _customer = default!;

  [IterationSetup]
  public void IterationSetup()
  {
    _customer = new Customer(Guid.NewGuid(), Fraud());
  }

  private Person Fraud()
  {
    return new Person(
      "Fraud",
      "Fraudowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "1234567890");
  }

  /// <summary>
  /// Test micro-benchmarkowy. Sprawdza jak szybki jest algorytm weryfikujący czy klient
  /// jest oszustem.
  /// </summary>
  [Benchmark]
  public Task ShouldProcessFraud()
  {
    var customerVerifier = new CustomerVerifier(
      Substitute.For<IBikVerificationService>(),
      Enumerable.Empty<IVerification>().ToList(),
      Substitute.For<IVerificationRepository>(),
      Substitute.For<IFraudAlertNotifier>(),
      Substitute.For<ILogger<CustomerVerifier>>());

    return customerVerifier.Verify(_customer, new CancellationToken());
  }

}