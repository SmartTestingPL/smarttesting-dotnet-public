using Core.Scoring.domain;
using Core.Scoring.Personal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BikService.Personal;

public class EfCoreOccupationRepository : 
  DbContext, 
  IOccupationRepository
{
  static EfCoreOccupationRepository()
  {
    NpgsqlConnection.GlobalTypeMapper.MapEnum<PersonalInformation.Occupations>();
  }

  private readonly string _connectionString;
  private readonly ILoggerFactory _loggerFactory;

  public DbSet<OccupationToScore> OccupationToScores { get; set; } = default!;

  public EfCoreOccupationRepository(IOptions<OccupationRepositoryOptions> dbOptions, ILoggerFactory loggerFactory)
  {
    _loggerFactory = loggerFactory;
    _connectionString = dbOptions.Value.ConnectionString;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(_connectionString)
      .UseLoggerFactory(_loggerFactory)
      .EnableSensitiveDataLogging();
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasPostgresEnum<PersonalInformation.Occupations>();
    modelBuilder.Entity<OccupationToScore>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Occupation).IsRequired();
      entity.Property(e => e.OccupationScore).IsRequired();
    });
    base.OnModelCreating(modelBuilder);
  }

  public Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    return OccupationToScores
      .ToDictionary(
        score => new PersonalInformation.Occupations?(score.Occupation),
        score => new Score(score.OccupationScore));
  }
}
