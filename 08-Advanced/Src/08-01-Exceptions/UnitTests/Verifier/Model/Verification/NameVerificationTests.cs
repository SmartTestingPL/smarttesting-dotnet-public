using System;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Model.Verification;

namespace UnitTests.Verifier.Model.Verification;

public class NameVerificationTests
{
  /// <summary>
  /// Test, w którym weryfikujemy czy został rzucony bardzo generyczny wyjątek
  /// <see cref="NullReferenceException"/>.
  /// 
  /// Test ten przechodzi nam przypadkowo, gdyż NRE leci w innym miejscu w kodzie
  /// niż się spodziewaliśmy.
  /// 
  /// Uruchamiając ten test nie widzimy żeby zalogowała nam się linijka z klasy
  /// _01_NameVerification...
  /// </summary>
  [Test]
  public void ShouldThrowAnExceptionWhenCheckingVerification()
  {
    new _01_NameVerification().Invoking(v => v.Passes(Anna()))
      .Should().ThrowExactly<NullReferenceException>();
  }

  /// <summary>
  /// Poprawiona wersja poprzedniego testu, gdzie tym razem zweryfikujemy
  /// zawartość wiadomości w rzuconym wyjątku.
  /// 
  /// Zakomentuj [Ignore()], żeby zobaczyć, że test się wysypuje, gdyż
  /// nie jest wołana nasza wersja NullReferenceException, tylko domyślna,
  /// w momencie wołania metody ToString() na wartości null
  /// zwracanej przez Person.GetGender().
  /// 
  /// Problem polega na tym, że w konstruktorze Person ktoś zapomniał ustawić
  /// pola _gender.
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldThrowAnExceptionWhenCheckingVerificationOnly()
  {
    new _01_NameVerification()
      .Invoking(v => v.Passes(Anna()))
      .Should().ThrowExactly<NullReferenceException>()
      .WithMessage("Name cannot be null");
  }

  /// <summary>
  /// W momencie, w którym nasza aplikacja rzuca wyjątki domenowe, wtedy nasz test
  /// może po prostu spróbować go wyłapać.
  /// 
  /// Zakomentuj [Ignore()], żeby zobaczyć, że test się wysypuje, gdyż wyjątek,
  /// który poleci to NullReferenceException, a nie <see cref="_04_VerificationException"/>.
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldFailVerificationWhenNameIsInvalid()
  {
    new _05_NameWithCustomExceptionVerification()
      .Invoking(v => v.Passes(Anna()))
      .Should().ThrowExactly<_04_VerificationException>();
  }

  /// <summary>
  /// Koncepcyjnie to samo co powyżej. Do zastosowania w momencie, w którym
  /// nie używacie bibliotek do asercji, które zawierają asercje na rzucanie wyjątków
  /// (coraz trudniej na taką trafić...).
  /// 
  /// Łapiemy w try {...} catch {...} wywołanie metody, która powinna rzucić wyjątek.
  /// Koniecznie należy zakończyć test niepowodzeniem, jeśli wyjątek nie zostanie rzucony!!!
  /// 
  /// W sekcji catch możemy wykonać dodatkowe asercje na rzuconym wyjątku.
  /// </summary>
  [Ignore("")]
  [Test]
  public void ShouldFailVerificationWhenNameIsInvalidAndAssertionIsDoneManually()
  {
    try
    {
      new _05_NameWithCustomExceptionVerification().Passes(Anna());
      Assert.Fail("Should fail the verification");
    }
    catch (_04_VerificationException ex)
    {
      // dodatkowe asercje jeśli potrzebne
    }
  }

  private Person Anna()
  {
    return new Person(
      "Anna",
      "Smith",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Female,
      "00000000000");
  }
}