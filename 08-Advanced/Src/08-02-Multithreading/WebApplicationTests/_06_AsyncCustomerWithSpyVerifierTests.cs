using System;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using FluentAssertions.Extensions;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Polly;
using WebApplication;

namespace WebApplicationTests;

/// <summary>
/// Test weryfikujący efekt uboczny w postaci wywołania
/// metody asynchronicznie.
/// </summary>
public class _06_AsyncCustomerWithSpyVerifierTests
{
  private IFraudAlertNotifier _verificationNotifier = default!;
  private ServiceProvider _container = default!;
  private _01_CustomerVerifier _verifier = default!;

  [SetUp]
  public void SetUp()
  {
    var containerBuilder = new ServiceCollection();
    Startup.AddDependenciesTo(containerBuilder);

    // Nadpisujemy rejestrację IFraudAlertNotifier "częściowego mocka".
    containerBuilder.AddSingleton<IFraudAlertNotifier>(
      context => Substitute.ForPartsOf<FakeVerificationNotifier>(
        context.GetRequiredService<ILogger<_04_VerificationNotifier>>()));

    _container = containerBuilder.BuildServiceProvider();
    _verificationNotifier = _container.GetRequiredService<IFraudAlertNotifier>();
    _verifier = _container.GetRequiredService<_01_CustomerVerifier>();
  }

  [TearDown]
  public void TearDown()
  {
    _container.Dispose();
  }

  /// <summary>
  /// W przeciwieństwie do Mockito (co pokazano w przykładzie Javowym),
  /// weryfikacja wywołania metody na częściowym mocku w NSubstitute
  /// nigdy nie woła implementacji z nadklasy. Co innego gdybyśmy chcieli
  /// ustawić wartość zwracaną za pomocą .Returns()
  /// - wtedy należałoby użyć metody .DoNotCallBase().
  /// Zobacz: https://nsubstitute.github.io/help/partial-subs/
  /// </summary>
  [Test]
  public Task ShouldDelegateWorkToASeparateThread()
  {
    _verifier.FoundFraud(new Customer(Guid.NewGuid(), TooYoungStefan()));

    return Policy.Handle<AssertionException>()
      .WaitAndRetryForeverAsync(_ => 100.Milliseconds())
      .ExecuteAsync(() =>
        _verificationNotifier.Received(1).FraudFound(
          Arg.Any<CustomerVerification>()));
  }

  private static Person TooYoungStefan()
  {
    return new Person("", "", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male, "0123456789");
  }
}

public abstract class FakeVerificationNotifier : _04_VerificationNotifier
{
  private readonly ILogger<_04_VerificationNotifier> _logger;

  protected FakeVerificationNotifier(ILogger<_04_VerificationNotifier> logger)
    : base(logger)
  {
    _logger = logger;
  }

  public override Task FraudFound(
    CustomerVerification customerVerification)
  {
    return Task.Run(() => { _logger.LogInformation("hello"); });
  }
}