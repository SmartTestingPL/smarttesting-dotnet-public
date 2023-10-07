using LoanOrders.Orders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoanOrders.Repository;

public class LoanOrderEntry
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }

  public LoanOrder? Item { get; set; }
}