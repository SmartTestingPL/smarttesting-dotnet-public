using Core.Scoring.Personal;

namespace BikService.Personal;

public interface IOccupationRepositoryInitialization
{
  Task Perform(IServiceProvider serviceProvider);
}

public class PostgreSqlOccupationRepositoryInitialization : IOccupationRepositoryInitialization
{
  public async Task Perform(IServiceProvider serviceProvider)
  {
    await using var scope = serviceProvider.CreateAsyncScope();
    var repo = scope.ServiceProvider.GetRequiredService<EfCoreOccupationRepository>();
    await repo.Database.EnsureCreatedAsync();
    repo.OccupationToScores.RemoveRange(repo.OccupationToScores);
    await repo.SaveChangesAsync();
    await repo.OccupationToScores.AddRangeAsync(
      new OccupationToScore { Occupation = PersonalInformation.Occupations.Programmer, OccupationScore = 1000 },
      new OccupationToScore { Occupation = PersonalInformation.Occupations.Lawyer, OccupationScore = 500 },
      new OccupationToScore { Occupation = PersonalInformation.Occupations.Doctor, OccupationScore = 250 },
      new OccupationToScore { Occupation = PersonalInformation.Occupations.Other, OccupationScore = 50 });
    await repo.SaveChangesAsync();
  }
}