using System;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using WebApplication;

namespace WebApplicationTests;

/// <summary>
/// Test weryfikujący efekt uboczny w postaci wywołania metody
/// asynchronicznie.
/// </summary>
public class _05_AsyncCustomerVerifierTests
{
  private IFraudAlertNotifier _verificationNotifier = default!;
  private ServiceProvider _container = default!;
  private _01_CustomerVerifier _verifier = default!;

  [SetUp]
  public void SetUp()
  {
    var containerBuilder = new ServiceCollection();
    Startup.AddDependenciesTo(containerBuilder);

    _verificationNotifier = Substitute.For<IFraudAlertNotifier>();
    // Nadpisujemy rejestrację IFraudAlertNotifier naszą własną.
    containerBuilder.AddSingleton(context => _verificationNotifier);
    _container = containerBuilder.BuildServiceProvider();
    _verifier = _container.GetRequiredService<_01_CustomerVerifier>();
  }

  [TearDown]
  public void TearDown()
  {
    _container.Dispose();
  }

  [Test]
  public Task ShouldNotifyAboutFraud()
  {
    _verifier.FoundFraud(new Customer(Guid.NewGuid(), TooYoungStefan()));

    return _verificationNotifier.Received(1).FraudFound(
      Arg.Any<CustomerVerification>());
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
}