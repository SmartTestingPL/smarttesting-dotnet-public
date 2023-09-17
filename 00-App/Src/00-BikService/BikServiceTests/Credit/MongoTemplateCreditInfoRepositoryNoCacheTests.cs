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
public class MongoTemplateCreditInfoRepositoryNoCacheTests
{
  private MongoDbContainer _mongoDbContainer = default!;
  private ServiceProvider _serviceProvider = default!;
  private const string MongoTestDbName = "test"; //domyœlna nazwa nadawana przez kontener Mongo;

  private ICreditInfoRepository CreditInfoRepository =>
    _serviceProvider.GetRequiredService<MongoTemplateCreditInfoRepository>();
  private MongoTemplate<CreditInfoDocument> MongoTemplate =>
    _serviceProvider.GetRequiredService<MongoTemplate<CreditInfoDocument>>();

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
  public async Task ShouldRetrieveCreditInfoFromDb()
  {
    var testPesel = new Pesel("09876543210");
    var creditInfoDocument = new CreditInfoDocument(CreditInfo(), testPesel);
    await MongoTemplate.Save(creditInfoDocument, p => p.Id == creditInfoDocument.Id);
    (await CreditInfoExistsForPesel(testPesel))
      .Should().BeTrue("There should be a test entry before running the test");

    var creditInfo = await CreditInfoRepository.FindCreditInfo(testPesel);

    creditInfo.Should().NotBeNull();
  }

  [Test]
  public async Task ShouldSaveCreditToDb()
  {
    var testPesel = new Pesel("12345678901");
    (await CreditInfoExistsForPesel(testPesel))
      .Should().BeFalse("There should not be a test entry before running the test");

    await CreditInfoRepository.SaveCreditInfo(testPesel, CreditInfo());

    (await CreditInfoExistsForPesel(testPesel)).Should().BeTrue("There should be a test entry after running the test");
  }

  private CreditInfo CreditInfo()
  {
    return new CreditInfo(decimal.One, decimal.Zero, Core.Scoring.Credit.CreditInfo.DebtPaymentHistoryStatus.IndividualUnpaidInstallments);
  }

  private async Task<bool> CreditInfoExistsForPesel(Pesel testPesel)
  {
    return await MongoTemplate.Exists(document => document.Pesel == testPesel);
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