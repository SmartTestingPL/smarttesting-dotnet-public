using System;
using System.Collections.Generic;
using Core.Customers;
using Core.Lib;
using Core.Verifier.Model;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;

namespace UnitTests.Verifier.Model;

public class CustomerVerifierTests
{
  [Test]
  public void ShouldCollectVerificationResults()
  {
    var verifier = new CustomerVerifier(new HashSet<IVerification>
    {
      new FirstVerification(), 
      new SecondVerification()
    });

    var verificationResults = verifier.Verify(
      new Customer(Guid.NewGuid(), TooYoungStefan()));

    verificationResults.Should().Equal(
      new VerificationResult("first", false),
      new VerificationResult("second", true));
  }

  private static Person TooYoungStefan()
  {
    return new Person(
      "Stefan",
      "Stefanowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "1234567890");
  }
}

internal class FirstVerification : IVerification
{
  public VerificationResult Passes(Person person)
  {
    return new VerificationResult(Name, false);
  }

  public string Name => "first";
}

internal class SecondVerification : IVerification
{
  public VerificationResult Passes(Person person)
  {
    return new VerificationResult(Name, true);
  }

  public string Name => "second";
}