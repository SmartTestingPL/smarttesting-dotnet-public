using Core.Scoring.Credit;
using Core.Scoring.domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BikService.Credit;

public class CreditInfoDocument
{
  [JsonConstructor]
  public CreditInfoDocument(CreditInfo creditInfo, Pesel pesel)
  {
    CreditInfo = creditInfo;
    Pesel = pesel;
    Id = ObjectId.GenerateNewId().ToString();
  }

  [BsonConstructor]
  public CreditInfoDocument(CreditInfo? creditInfo, Pesel pesel, string id)
  {
    CreditInfo = creditInfo;
    Pesel = pesel;
    Id = id;
  }

  public CreditInfo? CreditInfo { get; set; }
  public Pesel Pesel { get; set; }
  public string Id { get; set; }

  public override string ToString()
  {
    return $"CreditInfoDocument [creditInfo={CreditInfo}, pesel={Pesel}]";
  }
}
