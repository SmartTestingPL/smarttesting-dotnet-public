using Core.Customers;
using Core.Lib;
using Core.Verifier.Model.Verification;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;

namespace UnitTests.Verifier.Model.Verification;

public class IdentificationNumberVerificationTests
{
  [Test]
  public void ShouldReturnPositiveVerificationWhenGenderFemaleCorrespondsToIdNumber()
  {
    var verification = new IdentificationNumberVerification();

    var result = verification.Passes(AnnaTheWoman());

    result.Result.Should().BeTrue();
  }

  [Test]
  public void ShouldReturnNegativeVerificationWhenGenderFemaleDoesNotCorrespondToIdNumber()
  {
    var verification = new IdentificationNumberVerification();

    var result = verification.Passes(AnnaWithNonFemaleId());

    result.Result.Should().BeFalse();
  }

  [Test]
  public void ShouldReturnPositiveVerificationWhenGenderMaleCorrespondsToIdNumber()
  {
    var verification = new IdentificationNumberVerification();

    var result = verification.Passes(ZbigniewTheMan());

    result.Result.Should().BeTrue();
  }

  [Test]
  public void ShouldReturnNegativeVerificationWhenGenderMaleDoesNotCorrespondToIdNumber()
  {
    var verification = new IdentificationNumberVerification();

    var result = verification.Passes(ZbigniewWithNonMaleId());

    result.Result.Should().BeFalse();
  }

  private static Person AnnaTheWoman()
  {
    return new Person(
      "Anna", 
      "Annowska", 
      Now(), 
      Gender.Female, 
      "00000000020");
  }

  private static Person AnnaWithNonFemaleId()
  {
    return new Person(
      "Anna", 
      "Annowska", 
      Now(), 
      Gender.Female, 
      "00000000010");
  }

  private static Person ZbigniewTheMan()
  {
    return new Person(
      "Zbigniew", 
      "Zbigniewowski", 
      Now(), 
      Gender.Male, 
      "00000000010");
  }

  private static Person ZbigniewWithNonMaleId()
  {
    return new Person(
      "Zbigniew", 
      "Zbigniewowski", 
      Now(), 
      Gender.Male, 
      "00000000020");
  }

  private static Maybe<LocalDate> Now()
  {
    return Clocks.ZonedUtc.GetCurrentDate().Just();
  }
}