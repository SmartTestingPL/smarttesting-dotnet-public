using Core.Maybe;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

[Homework("Czy ten test na pewno jest czytelny? Co on w ogóle testuje? Czyżby wszystkie przypadki błędnych weryfikacji?")]
class BusinessRulesVerificationTests
{

  [Test]
  public void TestImpl()
  {
    var e = Substitute.For<IEventEmitter>();
    var i = Substitute.ForPartsOf<VerifierManagerImpl>();
    // Jan should fail
    i.VerifyName(Arg.Is<Person>(person => person.Name == "Jan")).Returns(false);
    var v = new BusinessRulesVerification(e, i);
    i.VerifyAddress(Arg.Any<Person>()).Returns(true);
    i.VerifyPhone(Arg.Any<Person>()).Returns(true);
    i.VerifyTaxInformation(Arg.Any<Person>()).Returns(true);
    var p = new Person("Jan", "Kowalski", Clocks.ZonedUtc.GetCurrentDate().Just(), Maybe<Gender>.Nothing,
      "12309279124123");
    i.VerifySurname(Arg.Any<Person>()).Returns(true);
    var passes = v.Passes(p);
    Assert.False(passes);
    i.Received(1).VerifyName(Arg.Is<Person>(person => person.Name == "Jan"));
    i.ClearSubstitute();

    i.VerifyName(Arg.Any<Person>()).Returns(true);
    i.VerifyAddress(Arg.Any<Person>()).Returns(false);
    passes = v.Passes(p);
    Assert.False(passes);
    i.ClearSubstitute();
    i.VerifyAddress(Arg.Any<Person>()).Returns(true);
    i.VerifyPhone(Arg.Any<Person>()).Returns(false);
    passes = v.Passes(p);
    Assert.False(passes);
    i.ClearSubstitute();
    i.VerifyPhone(Arg.Any<Person>()).Returns(true);
    i.VerifyTaxInformation(Arg.Any<Person>()).Returns(false);
    passes = v.Passes(p);
    Assert.False(passes);
  }
}