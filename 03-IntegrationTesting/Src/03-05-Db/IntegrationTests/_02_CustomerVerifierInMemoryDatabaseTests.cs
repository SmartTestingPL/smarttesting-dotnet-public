using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa pokazująca jak testując serwis aplikacyjny `CustomerVerifier`,
/// możemy użyć bazy danych w pamięci.
/// </summary>
public class _02_CustomerVerifierInMemoryDatabaseTests
{
  _02_InMemoryVerificationRepository _repository = default!;

  [SetUp]
  public void SetUp()
  {
    _repository = new _02_InMemoryVerificationRepository();
  }

  [Test]
  public async Task ShouldReturnCachedCustomerResultWhenCustomerAlreadyVerified()
  {
    var verifiedPerson = await GivenAnExistingVerifiedPerson();
    // Przed uruchomieniem metody do przetestowania,
    // upewniamy się, że w bazie danych istnieje wpis dla danego użytkownika
    _repository.FindByUserId(verifiedPerson.UserId).HasValue
      .Should().BeTrue("user must be saved in the database");

    var result = await CustomerVerifierWithExceptionThrowingBik()
      .Verify(new Customer(verifiedPerson.UserId, NonFraudPerson()));

    result.UserId.Should().Be(verifiedPerson.UserId, "must represent the same person");
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
  }

  [Test]
  public async Task ShouldCalculateCustomerResultWhenCustomerNotPreviouslyVerified()
  {
    var newPersonId = Guid.NewGuid();
    // Przed uruchomieniem metody do przetestowania,
    // upewniamy się, że w bazie danych NIE istnieje wpis dla danego użytkownika
    _repository.FindByUserId(newPersonId).HasValue.Should()
      .BeFalse("user must NOT be in the database");

    var result = await CustomerVerifierWithPassingBik()
      .Verify(new Customer(newPersonId, NonFraudPerson()));

    result.UserId.Should().Be(newPersonId, "must represent the same person");
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
    // Po uruchomieniu metody do przetestowania,
    // upewniamy się, że w bazie danych istnieje wpis dla danego użytkownika
    _repository.FindByUserId(newPersonId).HasValue
      .Should().BeTrue("Person must exist in the database");
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
  /// implementacją klienta BIK. Klient ten zawsze zwraca, że klient nie jest oszustem.
  /// <summary>
  /// <returns>implementacja <see cref="CustomerVerifier"/> z nadpisanym 
  /// serwisem kontaktującym się z BIKiem
  private CustomerVerifier CustomerVerifierWithPassingBik()
  {
    return new CustomerVerifier(
      new AlwaysPassingBikVerifier(), 
      new HashSet<IVerification>(), 
      _repository);
  }

  /// <summary>
  /// Zwracamy zweryfikowaną osobę, która zostaje też zapisana w bazie danych.
  /// </summary>
  /// <returns>zweryfikowana osoba</returns>
  private async Task<VerifiedPerson> GivenAnExistingVerifiedPerson()
  {
    var verifiedPerson = VerifiedNonFraud();
    await _repository.SaveAsync(verifiedPerson);
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
    return VerifiedPerson.CreateInstance(Guid.NewGuid(), 
      "1234567890", 
      VerificationStatus.VerificationPassed);
  }
}