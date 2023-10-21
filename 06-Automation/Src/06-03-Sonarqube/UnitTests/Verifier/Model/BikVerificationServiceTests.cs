using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier.Model;
using static TddXt.AnyRoot.Root;

namespace UnitTests.Verifier.Model;

/// <summary>
/// Testy do klasy <see cref="BikVerificationService"/>.
/// </summary>
public class BikVerificationServiceTests
{
  [Test]
  public async Task ShouldReturnSuccessfulVerification()
  {
    var service = new BikVerificationService("", Any.Instance<ILogger<BikVerificationService>>());

    var result = await service.Verify(
      new(Guid.NewGuid(), null!), 
      new());

    result.Passed().Should().BeTrue();
  }

  [Test]
  public async Task ShouldReturnFailedVerification()
  {
    var service = new ThrowingBikVerificationService();

    var result = await service.Verify(
      new(Guid.NewGuid(), null!),
      new());

    result.Passed().Should().BeFalse();
  }

  [Test]
  public void ShouldNotBlowUpDueToCyclomaticComplexity()
  {
    var service = new BikVerificationService("", Any.Instance<ILogger<BikVerificationService>>());

    var result = service.ComplexMethod(1, 2, 3);

    result.Should().Be(8);
  }
}

/// <summary>
/// Podklasa <see cref="BikVerificationService"/> na potrzeby testu.
/// Rzuca wyjątek za każdym razem gdy weryfikacja powinna przejść.
/// W odpowiedniku Javowym była to klasa anonimowa.
/// </summary>
internal class ThrowingBikVerificationService : BikVerificationService
{
  public ThrowingBikVerificationService() :
    base(string.Empty, Any.Instance<ILogger<BikVerificationService>>())
  {
  }

  protected override CustomerVerificationResult Pass(Customer customer)
  {
    throw new InvalidOperationException("Boom!");
  }
}