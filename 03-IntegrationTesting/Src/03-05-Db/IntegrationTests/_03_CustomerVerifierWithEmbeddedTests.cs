using System;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers;
using ProductionCode.Verifier.Customers.Verification;
using static IntegrationTests.EmbeddedVerificationRepository;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa zawiera testy klasy CustomerVerifier połączonej 
/// z repozytorium EntityFrameworka używającym osadzonej bazy SqLite.
///
/// Odpalamy w pamięci bazę SQLite i przekazujemy repozytorium do metody wytwórczej
/// kodu produkcyjnego, która utworzy nam obiekt klasy <see cref="CustomerVerifier"/>.
/// Moglibyśmy, podobnie jak w Javowym przykładzie, odpalić tutaj całą aplikację
/// Asp.Net Core'ową, ale prawie nic by to w teście nie zmieniło.
///
/// Migracje odpalane są na podstawie kodu klasy <see cref="EmbeddedVerificationRepository"/>.
/// </summary>
public class _03_CustomerVerifierWithEmbeddedTests
{
  /// <summary>
  /// Test weryfikujący, że wykorzystany zostanie ówczesnie zapisany rekord w
  /// bazie danych z rezultatem weryfikacji.
  /// 
  /// W innym przypadku doszłoby do próby odpytania BIKu i rzucony zostałby
  /// wyjątek.
  /// </summary>
  [Test]
  public async Task ShouldSuccessfullyVerifyACustomerWhenPreviouslyVerified()
  {
    await using var repository = await PrepareEmbeddedDatabase();
    var verifier = VerificationConfiguration.CreateCustomerVerifier(
      repository, new ExceptionThrowingBikVerifier());
    var zbigniew = Zbigniew();

    repository.FindByUserId(zbigniew.Guid).HasValue.Should().BeTrue();

    var result = await verifier.Verify(zbigniew);

    result.UserId.Should().Be(Zbigniew().Guid);
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
  }

  /// <summary>
  /// Przygotowanie repozytorium z osadzoną bazą danych SQLite
  /// </summary>
  /// <returns>repozytorium</returns>
  private static async Task<EmbeddedVerificationRepository> PrepareEmbeddedDatabase()
  {
    var repository = new EmbeddedVerificationRepository(CreateInMemoryDatabase());
    await repository.Database.EnsureCreatedAsync();
    await repository.SaveAsync(VerifiedPerson.CreateInstance(
        Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"),
        "1234567890",
        VerificationStatus.VerificationPassed
      )
    );
    return repository;
  }

  private static Customer Zbigniew()
  {
    return new Customer(Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"), YoungZbigniew());
  }

  private static Person YoungZbigniew()
  {
    return new Person("", "", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male, "18210116954");
  }
}