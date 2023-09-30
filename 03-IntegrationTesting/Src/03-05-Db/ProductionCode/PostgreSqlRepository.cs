using System;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using ProductionCode.Verifier.Customers;

namespace ProductionCode;

/// <summary>
/// Repozytorium Postgresowe, korzystające z Entity Frameworka.
/// W przeciwieństwie do wersji Javowej,
/// Tu migracje odbywają się na podstawie kodu.
/// </summary>
public class PostgreSqlRepository : DbContext, IVerificationRepository
{
  private readonly string _connectionString;

  public PostgreSqlRepository(string connectionString)
  {
    _connectionString = connectionString;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(_connectionString);
    base.OnConfiguring(optionsBuilder);
  }
    
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<VerifiedPerson>(entity =>
    {
      entity.ToTable("verified", "public");
    });
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
}