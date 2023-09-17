using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers;
using ProductionCode.Verifier.Customers.Verification;
using TddXt.AnyRoot.Invokable;
using static TddXt.AnyRoot.Root;

namespace UnitTests.Verifier.Customers;

/// <summary>
/// Klasa zawiera przykłady inicjalizacji w polach testowych,
/// przykład false-positive, przykład zastosowania Test Doubles.
///
/// Zestaw testów zawiera test na przypadek negatywny:
/// `CustomerVerifierTest.ShouldFailSimpleVerification`,
/// ale nie zawiera testów weryfikujących pozytywną weryfikację,
/// przez co testy nie wychwytują, że kod produkcyjny zwraca domyślną wartość
/// i brakuje implementacji logiki biznesowej. 
/// </summary>
public class CustomerVerifierTest
{
  // W zależności od używanego frameworku inicjalizacja w polach może być stanowa
  // dla metody testowej (świeży stan dla wywołania każdej metody) lub na cały test.
  // W PRZECIWIEŃSTWIE DO JUNITA, W NUNICIE PONIŻSZA LINIJKA WYKONA SIĘ RAZ NA WSZYSTKIE TESTY!
  private readonly CustomerVerifier _service = new(
    new TestVerificationService(),
    BuildVerifications(),
    new TestBadServiceWrapper());

  [Test]
  public async Task ShouldVerifyCorrectPerson()
  {
    // Given
    var customer = BuildCustomer();

    // When
    var result = await _service.Verify(customer, Any.CancellationToken());

    // Then
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
    result.UserId.Should().Be(customer.Guid);
  }

  // Test weryfikuje przypadek negatywnej weryfikacji, ale w klasie
  // zabrakło testu na pozytywną weryfikację klienta. Przez to testy
  // nie wychwytują, że kod produkcyjny zwraca domyślną wartość
  // i brakuje implementacji logiki biznesowej.
  [Test]
  public async Task ShouldFailSimpleVerification()
  {
    // Given
    var customer = BuildCustomer();
    var service = new CustomerVerifier(new TestVerificationService(),
      BuildSimpleVerification(), new TestBadServiceWrapper());

    // When
    var result = await service.Verify(customer, Any.CancellationToken());

    // Then
    result.Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  private IReadOnlyCollection<IVerification> BuildSimpleVerification()
  {
    return new HashSet<IVerification> { new SimpleVerification() };
  }

  private static IReadOnlyCollection<IVerification> BuildVerifications()
  {
    return new HashSet<IVerification>
    {
      new AgeVerification(),
      new IdentificationNumberVerification()
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
        Gender.Male,
        "96082812079"));
  }

  /// <summary>
  /// Implementacja testowa (Test Double) w celu uniknięcia kontaktowania się
  /// z zewnętrznym serwisem w testach jednostkowych.
  /// </summary>
  private class TestVerificationService : BikVerificationService
  {
    public TestVerificationService()
      : base("http://example.com", Any.Instance<ILogger<BikVerificationService>>())
    {
    }

    public override Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
    {
      return Task.FromResult(CustomerVerificationResult.Passed(customer.Guid));
    }
  }

  /// <summary>
  /// Implementacja testowa (Test Double) w celu uniknięcia kontaktowania się
  /// z zewnętrznym serwisem w testach jednostkowych.
  /// </summary>
  private class TestBadServiceWrapper : VeryBadVerificationServiceWrapper
  {
    public override Task<bool> Verify(CancellationToken cancellationToken)
    {
      // nie wykonujemy wszystkich tych kosztownych operacji w teście jednostkowym
      return Task.FromResult(true);
    }
  }
}