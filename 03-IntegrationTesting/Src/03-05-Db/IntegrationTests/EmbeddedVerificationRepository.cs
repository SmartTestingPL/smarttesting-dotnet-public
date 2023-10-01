using System;
using System.Data.Common;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductionCode.Verifier.Customers;

namespace IntegrationTests;

/// <summary>
/// Repozytorium oparte na osadzonej bazie danych SqLite.
/// Oryginał Javowy używa bazy H2.
/// </summary>
public class EmbeddedVerificationRepository : DbContext, IVerificationRepository
{
  private readonly DbConnection _dbConnection;

  public EmbeddedVerificationRepository(DbConnection dbConnection)
  {
    _dbConnection = dbConnection;
  }

  /// <summary>
  /// Konfigurujemy połączenie SqLite'owe
  /// </summary>
  /// <param name="optionsBuilder"></param>
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlite(_dbConnection);
    base.OnConfiguring(optionsBuilder);
  }
    
  /// <summary>
  /// Wykonujemy podstawową migrację - tworzymy tabelkę
  /// </summary>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<VerifiedPerson>().ToTable("verified", "test");
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<VerifiedPerson> Customers { get; set; } = default!;

  public Maybe<VerifiedPerson> FindByUserId(Guid userId)
  {
    return Customers.SingleMaybe(person => person.UserId == userId);
  }

  public async Task<VerifiedPerson> SaveAsync(VerifiedPerson verifiedPerson)
  {
    await Customers.AddAsync(verifiedPerson);
    await SaveChangesAsync();
    return verifiedPerson;
  }

  public static DbConnection CreateInMemoryDatabase()
  {
    var connection = new SqliteConnection("Filename=:memory:");
    connection.Open();
    return connection;
  }
}