using System;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Verifier.Customers;
using ProductionCode.Verifier.Customers.Verification;
using Testcontainers.PostgreSql;
using WireMock.Util;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa zawiera testy klasy <see cref="CustomerVerifier"/>
/// połączonej z repozytorium SQL.
/// Poprzez użycie Testcontainers uruchomi w kontenerze bazę danych PostgreSQL.
///
/// W tym teście widzimy, że uruchamiamy kontener z bazą danych i łączymy z nim
/// poprzez repozytorium logikę biznesową.
/// Na wstępie uruchomiona zostaje migracja podstawowa bazy danych z kodu
/// (<see cref="PostgreSqlRepository"/>), która utworzy tabelkę.
/// Następnie tworzymy kontener PostgreSQL za pomocą metody wytwórczej
/// w <see cref="TestContainersConfiguration"/>. Ta metoda wykona również
/// migrację testową polegającą na dodaniu użytkownika.
/// </summary>
public class _04_CustomerVerifierWithContainersTests
{
  /// <summary>
  /// Uruchomienie kontenera z bazą danych na losowym porcie przed uruchomieniem testów.
  /// </summary>
  private static PostgreSqlContainer _container = default!;

  /// <summary>
  /// Uruchamiamy kontener z bazą danych.
  /// </summary>
  [OneTimeSetUp]
  public async Task StartContainer()
  {
    //Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
    Environment.SetEnvironmentVariable("REAPER_DISABLED", "1");

    _container
      = new PostgreSqlBuilder()
        //Ustawienie wolnego portu. Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
        .WithPortBinding(5432, true)
        .WithImage("postgres:15.4")
        .WithDatabase("postgresik")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _container.StartAsync();
  }

  /// <summary>
  /// Zatrzymujemy kontener z bazą danych.
  /// </summary>
  [OneTimeTearDown]
  public async Task StopContainer()
  {
    await _container.DisposeAsync();
  }

  /// <summary>
  /// Test weryfikujący, że wykorzystany zostanie ówcześnie zapisany rekord
  /// w bazie danych z rezultatem weryfikacji.
  /// 
  /// W innym przypadku doszłoby do próby odpytania BIKu i rzucony zostałby
  /// wyjątek.
  /// </summary>
  [Test]
  public async Task ShouldSuccessfullyVerifyACustomerWhenPreviouslyVerified()
  {
    await using var repository = await TestContainersConfiguration
      .PreparePostgreSqlRepository(_container);

    var zbigniew = Zbigniew();
    repository.FindByUserId(zbigniew.Guid).HasValue.Should().BeTrue();

    var verifier = VerificationConfiguration.CreateCustomerVerifier(
      repository,
      new ExceptionThrowingBikVerifier());

    var result = await verifier.Verify(zbigniew);

    result.UserId.Should().Be(Zbigniew().Guid);
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
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