using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Verifier.Customers.Verification;

/// <summary>
/// Klasa zawiera przykłady różnych konwencji nazewniczych metod testowych.
/// Warto jest trzymać się jednej dla całej organizacji.
/// </summary>
public class NationalIdentificationNumberVerificationTest
{
  [Test]
  public void VerificationShouldPassForCorrectIdentificationNumber()
  {
    //given
    var person = BuildPerson(new LocalDate(1998, 3, 14), Gender.Female);
    var verification = new IdentificationNumberVerification();

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeTrue();
  }

  [Test]
  public void VerificationShouldFailForInconsistentGender()
  {
    //given
    var person = BuildPerson(new LocalDate(1998, 3, 14), Gender.Male);
    var verification = new IdentificationNumberVerification();

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  [Test]
  public void ShouldReturnFalseForWrongYearOfBirth()
  {
    //given
    var person = BuildPerson(new LocalDate(2000, 3, 14), Gender.Female);
    var verification = new IdentificationNumberVerification();

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  private static Person BuildPerson(LocalDate birthDate, Gender gender)
  {
    return new Person("John", "Doe", birthDate.Just(), gender, "98031416402");
  }
}