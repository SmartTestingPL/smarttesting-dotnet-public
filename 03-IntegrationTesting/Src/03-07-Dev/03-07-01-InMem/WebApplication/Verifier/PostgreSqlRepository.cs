using System;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace WebApplication.Verifier;

/// <summary>
/// Repozytorium Postgresowe, korzystające z Entity Frameworka.
/// W przeciwieństwie do wersji Javowej,
/// Tu migracje odbywają się na podstawie kodu.
/// </summary>
public class PostgreSqlRepository : DbContext, IVerificationRepository
{
  private readonly IOptions<PostgreSqlConfiguration> _config;

  public PostgreSqlRepository(IOptions<PostgreSqlConfiguration> config)
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
        optionsBuilder.UseNpgsql(_config.Value.ConnectionString,

          //konieczne dla kompatybilności z wersją 9.6 użytą w przykładzie
          options => options.SetPostgresVersion(new Version(9, 6)));
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

  public Task<int> Count()
  {
    return Customers.CountAsync();
  }

  public void EnsureExists()
  {
    Database.EnsureCreated();
  }
}