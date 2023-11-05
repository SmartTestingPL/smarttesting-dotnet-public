using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using Core.Verifier.Model.Verification;
using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using static TddXt.AnyRoot.Root;

namespace CoreTests;

/// <summary>
/// Testy do kodu reaktywnego z użyciem natywnych narzędzi testowych biblioteki Reactor.
/// </summary>
public class _08_RxCustomerVerifierTests
{
  /// <summary>
  /// Testujemy kod jednowątkowy.
  /// </summary>
  [Test]
  public async Task ShouldWorkWithRx()
  {
    var verifier = new _01_CustomerVerifier(
      Verifications(),
      Any.Instance<IFraudAlertNotifier>(),
      Any.Instance<IScheduler>());

    var observable = verifier.VerifyRx(
        new Customer(Guid.NewGuid(), TooYoungStefan()))
      .Select(r => r.VerificationName);
    var results = await observable.Take(3).ToList();

    results.Should().Equal("age", "id", "name");
  }

  /// <summary>
  /// Testujemy kod wielowątkowy.
  /// W przeciwieństwie do wersji Javowej,
  /// tutaj nie mamy do dyspozycji weryfikatora kroków,
  /// za to mamy testowego planistę (<see cref="TestScheduler"/>)
  /// </summary>
  [Test]
  public void ShouldWorkWithParallelRx()
  {
    var testScheduler = new TestScheduler();
    var results = new List<string>();
    var verifier = new _01_CustomerVerifier(
      Verifications(),
      Any.Instance<IFraudAlertNotifier>(),
      testScheduler);

    var observable = verifier.VerifyParallelRx(
        new Customer(Guid.NewGuid(), TooYoungStefan()))
      .Select(r => r.VerificationName);
    observable.Subscribe(s => results.Add(s));
    testScheduler.Start();

    results.Should().Equal("age", "id", "name");
  }

  private static Person TooYoungStefan()
  {
    return new Person(
      "", 
      "", 
      Clocks.ZonedUtc.GetCurrentDate().Just(), 
      Gender.Male, 
      "0123456789");
  }

  private static IReadOnlyCollection<IVerification> Verifications()
  {
    var emitter = Substitute.For<IEventEmitter>();
    return new IVerification[]
    {
      new AgeVerification(
        emitter,
        Any.Instance<ILogger<AgeVerification>>()),
      new IdentificationNumberVerification(
        emitter,
        Any.Instance<ILogger<IdentificationNumberVerification>>()),
      new NameVerification(
        emitter,
        Any.Instance<ILogger<NameVerification>>()),
    };
  }
}