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

namespace UnitTests.Verifier.Customers;

/// <summary>
/// Klasa zawiera
/// - przykłady zastosowania mocka w celu weryfikacji interakcji z obiektem typu EventEmitter,
/// - przykłady zastosowania buildera obiektów testowych,
/// - przykłady testowania komunikacji/interakcji.
/// </summary>
public class CustomerVerifierTest : CustomerTestBase
{
  private Customer _customer = default!;
  private CustomerVerifier _customerVerifier = default!;
  private IEventEmitter _eventEmitter = default!;

  [SetUp]
  public void SetUp()
  {
    _customer = BuildCustomer();
    // Tworzenie mocka obiektu typu IEventEmitter
    _eventEmitter = Substitute.For<IEventEmitter>();
    _customerVerifier = new CustomerVerifier(BuildVerifications(_eventEmitter));
  }

  private static IReadOnlyCollection<IVerification> BuildVerifications(IEventEmitter eventEmitter)
  {
    return new HashSet<IVerification>
    {
      new AgeVerification(eventEmitter),
      new IdentificationNumberVerification(eventEmitter),
      new NameVerification(eventEmitter)
    };
  }

  // Zastosowanie budowniczego w ustawianiu danych testowych.
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
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
    result.UserId.Should().Be(_customer.Guid);
  }

  // Testowanie komunikacji/interakcji
  [Test]
  public void ShouldEmitVerificationEvent()
  {
    _customerVerifier.Verify(_customer);

    // Weryfikacja interakcji - sprawdzamy, że metoda Emit(...) została wywołana 3 razy
    // z argumentem typu VerificationEvent, którego metoda Passed(...) zwraca true
    _eventEmitter.Received(3).Emit(Arg.Is<VerificationEvent>(@event => @event.Passed()));
  }

  // Metoda pomocnicza do ustawienia danych testowych.
  private Customer BuildCustomer()
  {
    return new Customer(
      Guid.NewGuid(),
      new Person(
        "John",
        "Smith",
        new LocalDate(1996, 8, 28).Just(),
        Gender.Male,
        "96082812079"));
  }
}