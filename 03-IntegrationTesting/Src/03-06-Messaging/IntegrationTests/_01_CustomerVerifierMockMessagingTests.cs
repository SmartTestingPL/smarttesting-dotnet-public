using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TddXt.XNSubstitute;
using WebApplication.Customers;
using WebApplication.Lib;
using WebApplication.Verifier;
using WebApplication.Verifier.Customers;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

/// <summary>
/// W tej klasie testowej
/// - piszemy test dla CustomerVerifiera i mockujemy
///   komponent wysyłający wiadomości,
/// - piszemy test dla komponentu wysyłającego wiadomość,
/// - piszemy test dla nasłuchiwacza wiadomości,
/// </summary>
public class _01_CustomerVerifierMockMessagingTests
{
  private IVerificationRepository _repository = default!;

  [SetUp]
  public void SetUp()
  {
    _repository = Substitute.For<IVerificationRepository>();
  }

  /// <summary>
  /// W tym teście testujemy serwis aplikacyjny, a mockujemy wysyłacza
  /// wiadomości (<see cref="IFraudAlertNotifier"/>). Nie testujemy żadnej integracji,
  /// działamy na mockach. Testy są szybkie - mogłyby być tak napisane
  /// dla głównej części naszej dziedziny.
  /// </summary>
  [Test]
  public async Task ShouldSendAMessageWithFraudDetailsWhenFoundAFraudUsingMocks()
  {
    var messaging = Substitute.For<IFraudAlertNotifier>();
    var fraud = Fraud();

    await AlwaysFailingCustomerVerifier(messaging)
      .Verify(new Customer(Guid.NewGuid(), fraud));

    messaging.Received(1).FraudFound(
      Arg.Is<CustomerVerification>(
        argument => NationalIdNumberAreEqual(fraud, argument)));
  }

  /// <summary>
  /// Przykład testu, w którym testujemy już sam komponent do wysyłki
  /// wiadomości. Mockujemy klienta do rabbita (RabbitTemplate) i
  /// weryfikujemy czy metoda na kliencie się wykonała.
  /// 
  /// Nie sprawdzamy żadnej integracji, test niewiele nam daje.
  /// </summary>
  [Test]
  public void ShouldSendAMessageUsingFraudDestination()
  {
    var verification = FraudCustomerVerification(Fraud(), Guid.NewGuid());
    var fraudDestination = Substitute.For<IFraudDestination>();
    new MessagingFraudAlertNotifier(
        fraudDestination, 
        Any.Instance<ILogger<MessagingFraudAlertNotifier>>())
      .FraudFound(verification);

    fraudDestination.Received(1).Send("fraudOutput", verification);
  }

  private static bool NationalIdNumberAreEqual(
    Person fraud, CustomerVerification argument)
  {
    return argument.Person.NationalIdentificationNumber 
           == fraud.NationalIdentificationNumber;
  }

  /// <summary>
  /// W tym teście weryfikujemy czy nasłuchiwacz na wiadomości, w momencie uzyskania
  /// wiadomości potrafi zapisać obiekt w bazie danych. Test ten nie integruje się
  /// z brokerem wiadomości więc nie mamy pewności czy potrafimy zdeserializować
  /// wiadomość. Z punktu widzenia nasłuchiwania zapis do bazy danych jest efektem
  /// ubocznym więc, możemy rozważyć użycie mocka.
  /// </summary>
  [Test]
  public async Task ShouldStoreFraudWhenReceivedOverMessaging()
  {
    var listener = new MessagingFraudListener(
      _repository,
      Any.Instance<ILogger<MessagingFraudListener>>());
    var guid = Guid.NewGuid();

    await listener.OnFraud(FraudCustomerVerification(Fraud(), guid));
    await _repository.Received(1).SaveAsync(PersonWith(guid));
  }

  private static VerifiedPerson PersonWith(Guid guid)
  {
    return Arg<VerifiedPerson>.That(p => p.UserId.Should().Be(guid));
  }

  private static CustomerVerification FraudCustomerVerification(
    Person fraud, 
    Guid userId)
  {
    return new CustomerVerification(
      fraud,
      new CustomerVerificationResult(
        userId,
        VerificationStatus.VerificationFailed));
  }

  private CustomerVerifier AlwaysFailingCustomerVerifier(IFraudAlertNotifier messaging)
  {
    return new CustomerVerifier(
      new HashSet<IVerification> {new AlwaysFailingVerification()},
      _repository,
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