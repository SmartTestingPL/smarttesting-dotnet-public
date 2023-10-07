using System.Threading.Tasks;
using Core.Maybe;
using Core.NullableReferenceTypesExtensions;
using LoanOrders.Customers;
using LoanOrders.Orders;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDb.Bson.NodaTime;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NodaTime;

namespace LoanOrders.Repository;

/// <summary>
/// Repozytorium do wykonywania operacji na wnioskach
/// o udzielenie pożyczki
/// </summary>
public class LoanOrderRepository
{
  private readonly IMongoCollection<LoanOrderEntry> _entries;

  public LoanOrderRepository(IOptions<MongoDbConfiguration> config)
  {
    var client = new MongoClient(config.Value.ConnectionString.OrThrow());
    var database = client.GetDatabase(config.Value.DatabaseName.OrThrow());
    _entries = database.GetCollection<LoanOrderEntry>("loanOrders");
  }

  public async Task<LoanOrder> FindById(string orderId)
  {
    using var loanOrderEntryCursor 
      = await _entries.FindAsync(entry => entry.Id == orderId).OrThrowAsync();
    var loanOrderEntry = await loanOrderEntryCursor.FirstOrDefaultAsync();
    return loanOrderEntry.Item.OrThrow();
  }

  public async Task<LoanOrderEntry> Save(LoanOrder loanOrder)
  {
    var id = ObjectId.GenerateNewId().ToString();
    var entry = new LoanOrderEntry
    {
      Id = id, 
      Item = loanOrder
    };

    await _entries.ReplaceOneAsync(
      p => p.Id == entry.Id,
      entry,
      new ReplaceOptions { IsUpsert = true });
    return entry;

  }

  static LoanOrderRepository()
  {
    BsonSerializer.RegisterSerializer(new LocalDateSerializer());
    BsonSerializer.RegisterSerializer(new MaybeLocalDateSerializer());
    BsonClassMap.RegisterClassMap<Maybe<LocalDate>>();
    BsonClassMap.RegisterClassMap<LoanOrderEntry>(cm =>
    {
      cm.AutoMap();
      cm.MapCreator(e => new LoanOrderEntry {Id = e.Id, Item = e.Item});
      cm.MapMember(entry => entry.Item);
    });
      
    BsonClassMap.RegisterClassMap<LoanOrder>(cm =>
    {
      cm.AutoMap();
      cm.MapCreator(o => new LoanOrder(o.Customer)
      {
        Amount = o.Amount,
        Guid = o.Guid,
        Commission = o.Commission,
        InterestRate = o.InterestRate,
        Status = o.Status
      });
      cm.MapMember(entry => entry.Customer);
      cm.MapMember(entry => entry.Amount);
      cm.MapMember(entry => entry.Commission);
      cm.MapMember(entry => entry.Guid);
      cm.MapMember(entry => entry.InterestRate);
      cm.MapMember(entry => entry.OrderDate);
      cm.MapMember(entry => entry.Status);
      cm.MapMember(entry => entry.Promotions);
    });

    BsonClassMap.RegisterClassMap<Customer>(cm =>
    {
      cm.AutoMap();
      cm.MapCreator(c => new Customer(c.Guid, c.Person));
      cm.MapMember(customer => customer.Guid);
      cm.MapMember(customer => customer.Person);
    });

    BsonClassMap.RegisterClassMap<Promotion>(cm =>
    {
      cm.AutoMap();
      cm.MapCreator(p => new Promotion(p.Name, p.Discount));
      cm.MapMember(customer => customer.Name);
      cm.MapMember(customer => customer.Discount);
    });

    BsonClassMap.RegisterClassMap<Person>(cm =>
    {
      cm.MapCreator(p => new Person(p.Name!, p.Surname!, p.DateOfBirth, p.Gender, p.NationalIdentificationNumber!));
      cm.AutoMap();
      cm.MapMember(person => person.Name);
      cm.MapMember(person => person.Surname);
      cm.MapMember(person => person.DateOfBirth);
      cm.MapMember(person => person.Gender);
      cm.MapMember(person => person.NationalIdentificationNumber);
    });
  }
}