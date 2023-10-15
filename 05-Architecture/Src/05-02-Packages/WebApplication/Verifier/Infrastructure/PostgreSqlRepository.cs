using System;
using System.Threading.Tasks;
using Core.Verifier;
using Core.Maybe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace WebApplication.Verifier.Infrastructure;

public class PostgreSqlRepository : DbContext, IVerificationRepository
{
  private readonly IOptions<PostgreSqlConfiguration> _postgresConfig;

  public PostgreSqlRepository(
    IOptions<PostgreSqlConfiguration> postgresConfig)
  {
    _postgresConfig = postgresConfig;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    Policy
      .Handle<Exception>()
      .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(3)
      ).Execute(() =>
      {
        optionsBuilder.UseNpgsql(_postgresConfig.Value.ConnectionString,

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
      entity.HasKey(e => e.UserId);
    });
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<VerifiedPerson> Customers { get; set; } = default!;

  public Maybe<VerifiedPerson> FindByUserId(Guid number)
  {
    return Customers.SingleMaybe(person => person.UserId == number);
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

  public void EnsureCreated()
  {
    Database.EnsureCreated();
  }
}