using System;
using System.Collections.Generic;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers;

public class CustomerVerifierTest : CustomerTestBase
{
  private Customer _customer = default!;
  private CustomerVerifier _customerVerifier = default!;
  private IEventEmitter _eventEmitter = default!;

  [SetUp]
  public void SetUp()
  {
    _customer = BuildCustomer();
    _eventEmitter = Substitute.For<IEventEmitter>();
    _customerVerifier = new CustomerVerifier(BuildVerifications(_eventEmitter));
  }

  [Test]
  public void ShouldVerifyCorrectPerson()
  {
    // Given
    _customer = Builder().WithNationalIdentificationNumber("80030818293")
      .WithDateOfBirth(1980, 3, 8)
      .WithGender(Gender.Male)
      .Build();

    // When
    var result = _customerVerifier.Verify(_customer);

    // Then
    result.GetStatus().Should().Be(CustomerVerificationResult.Status.VerificationPassed);
    result.GetUserId().Should().Be(_customer.Guid);
  }

  [Test]
  public void ShouldEmitVerificationEvent()
  {
    _customerVerifier.Verify(_customer);

    _eventEmitter.Received(2).Emit(Arg.Is<VerificationEvent>(@event => @event.Passed()));
  }

  private static IReadOnlyCollection<IVerification> BuildVerifications(IEventEmitter eventEmitter)
  {
    return new HashSet<IVerification>
    {
      new AgeVerification(eventEmitter),
      new NameVerification(eventEmitter)
    };
  }

  private Customer BuildCustomer()
  {
    return new Customer(
      Guid.NewGuid(),
      new Person(
        "John",
        "Smith",
        new LocalDate(1996, 8, 28).Just(),
        Gender.Male.Just(),
        "96082812079"));
  }
}