using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BikService;
using BikService.Credit;
using Core.Scoring.Credit;
using Core.Scoring.domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testcontainers.MongoDb;

namespace BikServiceTests.Credit;

// TODO: Dotyczy lekcji 03-05
public class MongoTemplateCreditInfoRepositoryCacheTests
{
  private MongoDbContainer _mongoDbContainer = default!;
  private ServiceProvider _serviceProvider = default!;
  private const string MongoTestDbName = "test"; //domyœlna nazwa nadawana przez kontener Mongo;
  private ICreditInfoRepository CreditInfoRepository => _serviceProvider.GetRequiredService<CachedCreditInfoRepository>();
  private MongoTemplate<CreditInfoDocument> MongoTemplate => _serviceProvider.GetRequiredService<MongoTemplate<CreditInfoDocument>>();
  
  [SetUp]
  public async Task SetUp()
  {
    _mongoDbContainer = new MongoDbBuilder()
      .WithCleanUp(true)
      .WithImage("mongo:6.0.8")
      .WithUsername("admin")
      .WithPassword("nimda")
      .Build();
    await _mongoDbContainer.StartAsync();
    _serviceProvider = await GetServiceProvider(
      _mongoDbContainer.GetConnectionString(), 
      MongoTestDbName);
  }

  [TearDown]
  public async Task TearDown()
  {
    await _serviceProvider.DisposeAsync();
    await _mongoDbContainer.DisposeAsync();
  }

  [Test]
  public async Task ShouldRetrieveCreditInfoFromCache()
  {
    var creditInfo = await GetCreditInfo(CreditInfoRepository);

    creditInfo.Should().NotBeNull("Data should be retrieved from the database");

    await DropCollectionCreditInfoDocuments();

    (await GetCreditInfo(CreditInfoRepository)).Should().NotBeNull("Data should be retrieved from the cache");
  }

  [Test]
  public async Task ShouldPutCreditInfoToCache()
  {
    var testPesel = new Pesel("09876543210");

    await CreditInfoRepository.SaveCreditInfo(testPesel, CreditInfo());

    await RemoveTestEntryFromDatabase(testPesel);

    var creditInfo = await CreditInfoRepository.FindCreditInfo(testPesel);

    creditInfo.Should().NotBeNull();
  }

  private async Task RemoveTestEntryFromDatabase(Pesel testPesel)
  {
    await MongoTemplate.Remove(PeselQuery(testPesel));
    (await CreditInfoExistsForPesel(testPesel))
      .Should().BeFalse("Test entry should be removed from db");
  }

  private CreditInfo CreditInfo()
  {
    return new CreditInfo(decimal.One, decimal.Zero, Core.Scoring.Credit.CreditInfo.DebtPaymentHistoryStatus.IndividualUnpaidInstallments);
  }

  private async Task<bool> CreditInfoExistsForPesel(Pesel testPesel)
  {
    return await MongoTemplate.Exists(PeselQuery(testPesel));
  }

  private static Expression<Func<CreditInfoDocument, bool>> PeselQuery(Pesel testPesel)
  {
    return document => document.Pesel == testPesel;
  }

  private async Task<CreditInfo?> GetCreditInfo(ICreditInfoRepository creditInfoRepository)
  {
    return await creditInfoRepository.FindCreditInfo(new Pesel("89050193724"));
  }

  private async Task DropCollectionCreditInfoDocuments()
  {
    (await MongoTemplate.CollectionExists()).Should().BeTrue();
    await MongoTemplate.DropCollection();
    (await MongoTemplate.CollectionExists()).Should().BeFalse();
  }

  private static async Task<ServiceProvider> GetServiceProvider(string connectionString, string database)
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddSingleton(ctx => Options.Create(new CreditDbOptions {
      ConnectionString = connectionString,
      DatabaseName = database
    }));
    serviceCollection.AddAppServices();
    var serviceProvider = serviceCollection.BuildServiceProvider();
    await serviceProvider.AddInitialMongoData();
    return serviceProvider;
  }

}