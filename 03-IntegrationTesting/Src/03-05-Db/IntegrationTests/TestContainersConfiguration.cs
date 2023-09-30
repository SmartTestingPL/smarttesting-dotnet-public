using System;
using System.Threading.Tasks;
using ProductionCode;
using ProductionCode.Verifier.Customers;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

internal static class TestContainersConfiguration
{
  /// <summary>
  /// Tworzy repozytorium PostgreSql podłączone do kontenera TestContainers
  /// poprzed łańcuch połączeniowy.
  /// Również wykonuje migrację testową - dodaje testowego użytkownika
  /// do bazy danych.
  /// </summary>
  /// <param name="postgreSqlContainer">kontener TestContainers</param>
  /// <returns>repozytorium</returns>
  public static async Task<PostgreSqlRepository> PreparePostgreSqlRepository(
    PostgreSqlContainer postgreSqlContainer)
  {
    var repository = new PostgreSqlRepository(
      postgreSqlContainer.GetConnectionString());
    await repository.Database.EnsureCreatedAsync();
      
    await repository.SaveAsync(
      VerifiedPerson.CreateInstance(
        Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"),
        "1234567890",
        VerificationStatus.VerificationPassed
      ));

    return repository;
  }
}