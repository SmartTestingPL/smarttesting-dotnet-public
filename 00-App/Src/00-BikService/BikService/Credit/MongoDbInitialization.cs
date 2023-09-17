namespace BikService.Credit;

public interface IMongoDbInitialization
{
  Task Perform(IServiceProvider serviceProvider);
}

public class MongoDbInitialization : IMongoDbInitialization
{
  public async Task Perform(IServiceProvider serviceProvider)
  {
    var mongoRepo = serviceProvider.GetRequiredService<ICreditInfoRepositoryForInitialization>();
    await new DatabaseChangelog(mongoRepo).Change();
  }
}