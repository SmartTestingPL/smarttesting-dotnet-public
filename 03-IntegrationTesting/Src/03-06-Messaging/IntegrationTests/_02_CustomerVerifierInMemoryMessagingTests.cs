using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using WebApplication.Customers;
using WebApplication.Lib;
using WebApplication.Verifier;
using WebApplication.Verifier.Customers;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

/// <summary>
/// W tej klasie testowej piszemy test dla serwisu CustomerVerifier, który
/// zamiast produkcyjnej instancji komponentu wysyłającego wiadomości,
/// użyje komponentu zawierającego kolejkę w pamięci.
/// </summary>
public class _02_CustomerVerifierInMemoryMessagingTests
{
  /// <summary>
  /// W tym teście wykorzystujemy implementację wysyłacza wiadomości z
  /// brokerem w formie kolejki w pamięci. W momencie, w którym zostaje
  /// wysłana wiadomość, ląduje ona w kolejce. Wykorzystując konkretną
  /// implementację (a nie interfejs) jesteśmy w stanie wyciągnąć tę
  /// wiadomość i ocenić jej zawartość.
  /// 
  /// Testy są szybkie - mogłyby być tak napisane
  /// dla głównej części naszej dziedziny.
  /// </summary>
  [Test]
  public async Task ShouldSendAMessageWithFraudDetailsWhenFoundAFraud()
  {
    using var messaging = new InMemoryMessaging();
    var fraud = Fraud();

    await AlwaysFailingCustomerVerifier(messaging)
      .Verify(new Customer(Guid.NewGuid(), fraud));

    var sentVerification = messaging.Poll();

    sentVerification.HasValue.Should().BeTrue();
    sentVerification.Value().Person.NationalIdentificationNumber
      .Should().Be(fraud.NationalIdentificationNumber);
  }

  private static CustomerVerifier AlwaysFailingCustomerVerifier(IFraudAlertNotifier messaging)
  {
    return new CustomerVerifier(
      new HashSet<IVerification> { new AlwaysFailingVerification() },
      Any.Instance<IVerificationRepository>(),
      messaging);
  }

  private static Person Fraud()
  {
    return new Person(
      "Fraud",
      "Fraudowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "1234567890");
  }
}