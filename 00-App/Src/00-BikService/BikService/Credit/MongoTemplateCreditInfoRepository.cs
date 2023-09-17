using Core.Scoring.Credit;
using Core.Scoring.domain;
using MongoDB.Bson.Serialization;

namespace BikService.Credit;

public class MongoTemplateCreditInfoRepository : ICreditInfoRepository, ICreditInfoRepositoryForInitialization
{
  private readonly ILogger<MongoTemplateCreditInfoRepository> _log;
  private readonly MongoTemplate<CreditInfoDocument> _mongoTemplate;

  public MongoTemplateCreditInfoRepository(
    ILogger<MongoTemplateCreditInfoRepository> log,
    MongoTemplate<CreditInfoDocument> mongoTemplate)
  {
    _log = log;
    _mongoTemplate = mongoTemplate;
  }

  public async Task<CreditInfo?> FindCreditInfo(Pesel pesel)
  {
    _log.LogInformation($"Getting credit info from Mongo for [{pesel}]");
    var creditInfoDocument = await _mongoTemplate.GetOneBy(document => document.Pesel == pesel);
    _log.LogInformation($"Found credit info [{creditInfoDocument}] for [{pesel}]");

    if (creditInfoDocument == null)
    {
      return null;
    }

    return creditInfoDocument.CreditInfo;
  }

  public async Task<CreditInfo> SaveCreditInfo(Pesel pesel, CreditInfo creditInfo)
  {
    _log.LogInformation($"Storing credit info [{creditInfo}] for [{pesel}]");

    var creditInfoDocument = new CreditInfoDocument(creditInfo, pesel);
    await _mongoTemplate.Save(creditInfoDocument, p => p.Id == creditInfoDocument.Id);
    return creditInfo;
  }

  public async Task Save(CreditInfoDocument creditInfoDocument)
  {
    _log.LogInformation($"Storing credit info [{creditInfoDocument.CreditInfo}] for [{creditInfoDocument.Pesel}]");
    await _mongoTemplate.Save(creditInfoDocument, p => p.Id == creditInfoDocument.Id);
  }

  public async Task Clear()
  {
    await _mongoTemplate.DropCollection();
  }

  static MongoTemplateCreditInfoRepository()
  {
    if (!BsonClassMap.IsClassMapRegistered(typeof(CreditInfoDocument)))
    {
      BsonClassMap.RegisterClassMap<CreditInfoDocument>(cm =>
      {
        cm.AutoMap();
        cm.MapCreator(d => new CreditInfoDocument(d.CreditInfo, d.Pesel, d.Id));
        cm.MapMember(entry => entry.Pesel);
        cm.MapMember(entry => entry.CreditInfo);
      });
    }
    if (!BsonClassMap.IsClassMapRegistered(typeof(Pesel)))
    {
      BsonClassMap.RegisterClassMap<Pesel>(cm =>
      {
        cm.AutoMap();
        cm.MapCreator(e => new Pesel(e.Value));
        cm.MapMember(entry => entry.Value);
      });
    }
    if (!BsonClassMap.IsClassMapRegistered(typeof(CreditInfo)))
    {
      BsonClassMap.RegisterClassMap<CreditInfo>(cm =>
      {
        cm.AutoMap();
        cm.MapCreator(e => new CreditInfo(e.CurrentDebt, e.CurrentLivingCosts, e.DebtPaymentHistory));
        cm.MapMember(entry => entry.CurrentDebt);
        cm.MapMember(entry => entry.CurrentLivingCosts);
        cm.MapMember(entry => entry.DebtPaymentHistory);
      });
    }
  }
}