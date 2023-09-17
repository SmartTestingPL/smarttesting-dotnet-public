using System;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace FraudDetection.Verifier;

/// <summary>
/// Repozytorium Postgresowe, korzystające z Entity Frameworka.
/// W przeciwieństwie do wersji Javowej,
/// Tu migracje odbywają się na podstawie kodu.
/// </summary>
public class PostgreSqlRepository : DbContext, IVerificationRepository
{
  private readonly IOptions<PostgreSqlOptions> _config;

  public PostgreSqlRepository(IOptions<PostgreSqlOptions> config)
  {
    _config = config;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    Policy
      .Handle<Exception>()
      .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(3))
      .Execute(() =>
      {
        optionsBuilder.UseNpgsql(_config.Value.ConnectionString);
        base.OnConfiguring(optionsBuilder);
      });
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

  public async Task<int> Count()
  {
    return await Customers.CountAsync();
  }

  public void EnsureExists()
  {
    Database.EnsureCreated();
    Customers.RemoveRange(Customers);
    Customers.Add(VerifiedPerson.CreateInstance(
        Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"),
        "1234567890",
        VerificationStatus.VerificationPassed
      )
    );
    SaveChanges();
  }
}