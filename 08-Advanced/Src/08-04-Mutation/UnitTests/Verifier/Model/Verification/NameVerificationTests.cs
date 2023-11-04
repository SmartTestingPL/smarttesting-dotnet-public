using Core.Customers;
using Core.Lib;
using Core.Verifier.Model.Verification;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;

namespace UnitTests.Verifier.Model.Verification;

public class NameVerificationTests
{
  [Test]
  public void ShouldReturnPositiveResultWhenNameIsNotBlank()
  {
    var verification = new NameVerification();

    var result = verification.Passes(NamelessPerson());

    result.Result.Should().BeFalse();
  }

  private static Person NamelessPerson()
  {
    return new Person(
      string.Empty,
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "1234567890");
  }
}