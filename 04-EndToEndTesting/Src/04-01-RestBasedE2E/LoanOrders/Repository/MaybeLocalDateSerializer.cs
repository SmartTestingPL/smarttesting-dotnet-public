using System;
using Core.Maybe;
using MongoDB.Bson;
using MongoDb.Bson.NodaTime;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime;

namespace LoanOrders.Repository;

public class MaybeLocalDateSerializer : SerializerBase<Maybe<LocalDate>>
{
  private readonly LocalDateSerializer _localDateSerializer = new LocalDateSerializer();

  public override Maybe<LocalDate> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
  {
    var currentBsonType = context.Reader.GetCurrentBsonType();
    switch (currentBsonType)
    {
      case BsonType.String:
        return _localDateSerializer.Deserialize(context, args).Just();
      case BsonType.Null:
        return default;
      default:
        throw new NotSupportedException(string.Format("Cannot convert a {0} to a {1}.", currentBsonType,
          typeof(LocalDate).Name));
    }
  }

  public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Maybe<LocalDate> maybe)
  {
    if (maybe.IsNothing())
    {
      context.Writer.WriteNull();
    }
    else
    {
      _localDateSerializer.Serialize(context, args, maybe.Value());
    }
  }
}