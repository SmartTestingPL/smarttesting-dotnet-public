using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using WireMock.Server;

namespace BikServiceTests.Acceptance;

public class BikWebAppForAcceptanceTests : BikWebApp
{
  private readonly WireMockServer _socialService;
  private readonly WireMockServer _personalService;
  private readonly WireMockServer _monthlyCostService;
  private readonly WireMockServer _incomeService;
  private readonly RabbitMqContainer _rabbitMqTestcontainer;
  private readonly PostgreSqlContainer _postgreSqlTestcontainer;
  private readonly MongoDbContainer _mongoDbTestcontainer;

  public BikWebAppForAcceptanceTests(
    WireMockServer socialService,
    WireMockServer personalService,
    WireMockServer monthlyCostService,
    WireMockServer incomeService,
    RabbitMqContainer rabbitMqTestcontainer,
    PostgreSqlContainer postgreSqlTestcontainer,
    MongoDbContainer mongoDbTestcontainer)
  {
    _socialService = socialService;
    _personalService = personalService;
    _monthlyCostService = monthlyCostService;
    _incomeService = incomeService;
    _rabbitMqTestcontainer = rabbitMqTestcontainer;
    _postgreSqlTestcontainer = postgreSqlTestcontainer;
    _mongoDbTestcontainer = mongoDbTestcontainer;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    builder.ConfigureAppConfiguration(configurationBuilder =>
      configurationBuilder.AddInMemoryCollection(
        new Dictionary<string, string>
        {
          ["SocialService:BaseUrl"] = _socialService.Urls.Single(),
          ["PersonalService:BaseUrl"] = _personalService.Urls.Single(),
          ["MonthlyCostService:BaseUrl"] = _monthlyCostService.Urls.Single(),
          ["IncomeService:BaseUrl"] = _incomeService.Urls.Single(),
          ["RabbitMq:ConnectionString"] = _rabbitMqTestcontainer.GetConnectionString(),
          ["OccupationRepository:ConnectionString"] = _postgreSqlTestcontainer.GetConnectionString(),
          ["CreditDb:ConnectionString"] = _mongoDbTestcontainer.GetConnectionString(),
          ["CreditDb:DatabaseName"] = "test", //domyślna nazwa nadawana przez kontener Mongo
        }));
  }


}