using System.Linq.Expressions;
using MongoDB.Driver;

namespace BikService.Credit;

public class MongoTemplate<T>
{
  private readonly IMongoDatabase _database;
  private readonly IMongoCollection<T> _mongoCollection;

  public MongoTemplate(string connectionString, string databaseName)
  {
    var client = new MongoClient(connectionString);
    _database = client.GetDatabase(databaseName);
    _mongoCollection = _database.GetCollection<T>(typeof(T).Name);
  }

  public async Task Save(T item, Expression<Func<T, bool>> condition)
  {
    await _mongoCollection.ReplaceOneAsync(
      condition,
      item,
      new ReplaceOptions { IsUpsert = true });
  }

  public async Task<T?> GetOneBy(Expression<Func<T, bool>> condition)
  {
    return await _mongoCollection.Find(condition)
      .Limit(1).FirstOrDefaultAsync();
  }

  public async Task Remove(Expression<Func<T, bool>> condition)
  {
    await _mongoCollection.DeleteOneAsync(condition);
  }

  public async Task<bool> Exists(Expression<Func<T, bool>> condition)
  {
    return await _mongoCollection.Find(condition).AnyAsync();
  }

  public async Task<bool> CollectionExists()
  {
    return (await _database.ListCollectionNames().ToListAsync()).Contains(typeof(T).Name);
  }

  public async Task DropCollection()
  {
    await _database.DropCollectionAsync(typeof(T).Name);
  }
}