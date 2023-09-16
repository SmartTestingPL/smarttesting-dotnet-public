using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier.Customers;

namespace UnitTests.Verifier.Customers;

/// <summary>
/// Test zawiera przykład `false pass`.
/// </summary>
class SimpleVerificationTest
{
  /// <summary>
  /// Przykład `false pass`. Test weryfikuje nie to co trzeba
  /// (jakieś pole na obiekcie, zamiast zwracanej wartości),
  /// przez co przechodzi, mimo że właściwa implementacja nie została dodana.
  /// </summary>
  [Test]
  public void ShouldFailSimpleVerificationFalsePass()
  {
    // Given
    var verification = new SimpleVerification();
    var person = new Person(
      "John", 
      "Smith",
      new LocalDate(1996, 8, 28).Just(), 
      Gender.Male,
      "96082812079");

    // When
    verification.Passes(person);

    // Then
    verification.VerificationPasses().Should().BeTrue();
  }
}