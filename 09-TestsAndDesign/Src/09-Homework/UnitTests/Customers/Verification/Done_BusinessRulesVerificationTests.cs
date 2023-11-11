using System;
using System.Collections.Generic;
using FluentAssertions;
using Core.Maybe;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;

namespace UnitTests.Customers.Verification;

/// <summary>
/// Pierwotny test był bardzo źle napisany. Nie dość, że nie wiemy, co testujemy patrząc na nazwę 
/// metody testowej to nawet nie wiemy gdzie jest sekcja when. Kod jest bardzo nieczytelny i robi
/// stanowczo za dużo. Używa też niepotrzebnych konkretnych weryfikacji
/// </summary>
public class Done_BusinessRulesVerificationTests
{
  private IEventEmitter _emitter = default!;
  private VerifierManagerImpl _manager = default!;

  /// <summary>
  /// Przykład wykorzystania mapy do testów parametryzowanych. W zależności od tego, który 
  /// przypadek błędnych weryfikacji chcemy przetestować (Adres, Imię, Nazwisko itd.) wybieramy
  /// odpowiednią metodę ustawiającej stan mocka.
  /// </summary>
  private static readonly Dictionary<string, Action<VerifierManagerImpl>> FaultyVerificationSetup = new()
  {
    ["Address"] = manager => manager.VerifyAddress(Arg.Any<Person>()).Returns(false),
    ["Name"] = manager => manager.VerifyName(Arg.Any<Person>()).Returns(false),
    ["Surname"] = manager => manager.VerifySurname(Arg.Any<Person>()).Returns(false),
    ["Phone"] = manager => manager.VerifyPhone(Arg.Any<Person>()).Returns(false),
    ["Tax"] = manager => manager.VerifyTaxInformation(Arg.Any<Person>()).Returns(false)
  };

  /// <summary>
  /// Przed każdym testem ustawiamy domyślne wartości mocka na true.
  /// </summary>
  [SetUp]
  public void Setup()
  {
    _emitter = Substitute.For<IEventEmitter>();
    _manager = Substitute.ForPartsOf<VerifierManagerImpl>();

    _manager.VerifyAddress(Arg.Any<Person>()).Returns(true);
    _manager.VerifyName(Arg.Any<Person>()).Returns(true);
    _manager.VerifySurname(Arg.Any<Person>()).Returns(true);
    _manager.VerifyPhone(Arg.Any<Person>()).Returns(true);
    _manager.VerifyTaxInformation(Arg.Any<Person>()).Returns(true);
  }

  [Test]
  public void ShouldVerifyPositivelyAPersonWhenDefaultVerifierManagerRulesAreApplied()
  {
    var verification = new BusinessRulesVerification(_emitter, new VerifierManagerImpl());

    verification.Passes(Person()).Should().BeTrue();
    _emitter.Received(1).Emit(new VerificationEvent(true));
  }

  /// <summary>
  /// Test parametryzowany przypadków negatywnych. Na podstawie typu weryfikacji ustawi mocka w odpowiednim stanie.
  /// 
  /// <param name="verificationType">verificationType typ weryfikacji, który oczekujemy, że się nie zwaliduje</param>
  /// </summary>
  [TestCase("Address")]
  [TestCase("Name")]
  [TestCase("Surname")]
  [TestCase("Phone")]
  [TestCase("Tax")]
  public void ShouldVerifyNegativelyWhenAPersonHasAnIllegalVerificationOfType(string verificationType)
  {
    FaultyVerificationSetup[verificationType](_manager);

    var verification = new BusinessRulesVerification(_emitter, _manager);

    verification.Passes(Person()).Should().BeFalse();
    _emitter.Received(1).Emit(new VerificationEvent(false));
  }

  private static Person Person()
  {
    return new Person("J", "K", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male.Just(), "1234567890");
  }
}