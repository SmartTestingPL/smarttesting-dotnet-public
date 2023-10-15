using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Lib;
using Core.Maybe;
using Core.Verifier;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace WebApplicationTests.Verifier.Model;

public class CustomerVerifierTests
{
  [Test]
  public async Task ShouldNotThrowWhenVerifying()
  {
    //GIVEN
    var customerVerifier = new CustomerVerifier(
      Substitute.For<IBikVerificationService>(),
      Enumerable.Empty<IVerification>().ToList(),
      Substitute.For<IVerificationRepository>(),
      Substitute.For<IFraudAlertNotifier>(),
      Substitute.For<ILogger<CustomerVerifier>>());

    var customer = new Customer(Guid.NewGuid(),
      new Person("Fraud",
        "Fraudowski",
        Clocks.ZonedUtc.GetCurrentDate().Just(),
        Gender.Male,
        "1234567890"));

    //WHEN - THEN
    await customerVerifier.Awaiting(v => v.Verify(customer, new CancellationToken()))
      .Should().NotThrowAsync();
  }
}