using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers;
using TddXt.XNSubstitute;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa pokazująca jak testując serwis aplikacyjny `CustomerVerifier`,
/// możemy zamockować komunikację z bazą danych.
/// </summary>
public class _01_CustomerVerifierMocksDatabaseTests
{
  IVerificationRepository _repository = default!;

  [SetUp]
  public void SetUp()
  {
    _repository = Substitute.For<IVerificationRepository>();
  }

  /// <summary>
  /// W przypadku ówczesnego zapisu klienta w bazie danych, chcemy się upewnić, że
  /// nie dojdzie do ponownego zapisu klienta w bazie danych.
  /// </summary>
  [Test]
  public async Task ShouldReturnStoredCustomerResultWhenCustomerAlreadyVerified()
  {
    var verifiedPerson = GivenAnExistingVerifiedPerson();

    var result = await CustomerVerifierWithExceptionThrowingBik()
      .Verify(new Customer(verifiedPerson.UserId, NonFraudPerson()));

    result.UserId.Should().Be(verifiedPerson.UserId, "must represent the same person");
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
      
    // chcemy się upewnić, że nie doszło do zapisu w bazie danych
    await _repository.DidNotReceive().SaveAsync(Arg.Any<VerifiedPerson>());
  }

  /// <summary>
  /// W przypadku braku zapisu klienta w bazie danych, chcemy się upewnić,
  /// że dojdzie do zapisu w bazie danych.
  /// </summary>
  [Test]
  public async Task ShouldCalculateCustomerResultWhenCustomerNotPreviouslyVerified()
  {
    var newPersonId = Guid.NewGuid();

    var result = await CustomerVerifierWithPassingBik()
      .Verify(new Customer(newPersonId, NonFraudPerson()));

    result.UserId.Should().Be(newPersonId, "must represent the same person");
    result.Status.Should().Be(VerificationStatus.VerificationPassed);

    // chcemy się upewnić, że doszło do zapisu w bazie danych
    await _repository.Received(1).SaveAsync(VerifiedPersonWith(newPersonId));
  }

  /// <summary>
  /// Testowa implementacja serwisu <see cref="CustomerVerifier"/>.
  /// Jeśli zapis w bazie miał już miejsce to nie powinniśmy wołać BIKa.
  /// Jeśli BIK zostanie wywołany to chcemy rzucić wyjątek.
  /// </summary>
  /// <returns>implementacja <see cref="CustomerVerifier"/> z nadpisanym serwisem
  /// kontaktującym się z BIKiem</returns>
  private CustomerVerifier CustomerVerifierWithExceptionThrowingBik()
  {
    return new CustomerVerifier(
      new ExceptionThrowingBikVerifier(), 
      new HashSet<IVerification>(), 
      _repository);
  }

  /// <summary>
  /// Testowa implementacja serwisu <see cref="CustomerVerifier"/>, z nadpisaną
  /// implementacją klienta BIK. Klient ten zawsze zwraca, że
  /// </summary>
  /// <returns>implementacja <see cref="CustomerVerifier"/> z nadpisanym
  /// serwisem kontaktującym się z BIKiem</returns>
  private CustomerVerifier CustomerVerifierWithPassingBik()
  {
    return new CustomerVerifier(
      new AlwaysPassingBikVerifier(),
      new HashSet<IVerification>(), 
      _repository);
  }

  private VerifiedPerson GivenAnExistingVerifiedPerson()
  {
    var verifiedPerson = VerifiedNonFraud();
    // Symulujemy, że osoba została zapisana w bazie danych wcześniej
    _repository.FindByUserId(verifiedPerson.UserId).Returns(verifiedPerson.Just());
    return verifiedPerson;
  }

  private Person NonFraudPerson()
  {
    return new Person(
      "Ucziwy",
      "Ucziwowski",
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "1234567890");
  }

  private VerifiedPerson VerifiedNonFraud()
  {
    return VerifiedPerson.CreateInstance(
      Guid.NewGuid(), 
      "1234567890", 
      VerificationStatus.VerificationPassed);
  }

  private static VerifiedPerson VerifiedPersonWith(Guid newPersonId)
  {
    return Arg<VerifiedPerson>.That(p => p.UserId.Should().Be(newPersonId));
  }
}