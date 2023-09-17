using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using WireMock.Matchers;
using WireMock.Server;
using Request = WireMock.RequestBuilders.Request;
using Response = WireMock.ResponseBuilders.Response;

namespace BikServiceTests.Acceptance;

public class Infrastructure
{
  protected RabbitMqContainer Rabbit = default!;
  protected MongoDbContainer MongoDbContainer = default!;
  protected PostgreSqlContainer PostgreSql = default!;
  protected WireMockServer IncomeService = default!;
  protected WireMockServer MonthlyCostService = default!;
  protected WireMockServer PersonalService = default!;
  protected WireMockServer SocialService = default!;

  [OneTimeSetUp]
  public static async Task OneTimeSetUp()
  {
    Environment.SetEnvironmentVariable("REAPER_DISABLED", "1");
  }

  [SetUp]
  public void InitializeContainers()
  {
    Rabbit = new RabbitMqBuilder()
      .WithCleanUp(true)
      .WithImage("rabbitmq:3.7.25-management-alpine")
      .WithUsername("admin")
      .WithPassword("nimda")
      .Build();

    MongoDbContainer
      = new MongoDbBuilder()
        .WithCleanUp(true)
        .WithImage("mongo:6.0.8")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();

    PostgreSql 
      = new PostgreSqlBuilder()
        //Ustawienie wolnego portu. Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
        .WithPortBinding(5432, true)
        .WithImage("postgres:15.4")
        .WithDatabase(Guid.NewGuid().ToString("N"))
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
  }

  protected BikWebAppForAcceptanceTests StartApp()
  {
    return new BikWebAppForAcceptanceTests(
      SocialService,
      PersonalService,
      MonthlyCostService,
      IncomeService,
      Rabbit,
      PostgreSql,
      MongoDbContainer);
  }

  public void StartHttpServers()
  {
    IncomeService = WireMockServer.Start();
    MonthlyCostService = WireMockServer.Start();
    PersonalService = WireMockServer.Start();
    SocialService = WireMockServer.Start();
  }

  public void StubServices()
  {
    StubIncomeService();
    StubMonthlyCostService();
    StubPersonalService();
    StubSocialService();
  }

  public void ShutdownServices()
  {
    IncomeService.Stop();
    MonthlyCostService.Stop();
    PersonalService.Stop();
    SocialService.Stop();
  }

  private void StubSocialService()
  {
    SocialService.Given(Request.Create().WithUrl(new RegexMatcher("/[0-9]{11}")).UsingGet())
      .AtPriority(int.MaxValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(request => JsonSerializer.Serialize(new
        {
          noOfDependants = 2,
          noOfPeopleInTheHousehold = 3,
          maritalStatus = "Married",
          contractType = "EmploymentContract"
        })).WithHeader("Content-Type", MediaTypeNames.Application.Json)
      );
    SocialService.Given(Request.Create().WithUrl(new RegexMatcher("/12345678901")).UsingGet())
      .AtPriority(int.MinValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(request => JsonSerializer.Serialize(new
        {
          noOfDependants = 20,
          noOfPeopleInTheHousehold = 30,
          maritalStatus = "Married",
          contractType = "Unemployed"
        })).WithHeader("Content-Type", MediaTypeNames.Application.Json)
      );
  }

  private void StubPersonalService()
  {
    PersonalService.Given(Request.Create().WithUrl(new RegexMatcher("/[0-9]{11}")))
      .AtPriority(int.MaxValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(request => JsonSerializer.Serialize(new
        {
          education = "High",
          yearsOfWorkExperience = 10,
          occupation = "Programmer"
        })).WithHeader("Content-Type", MediaTypeNames.Application.Json)
      );

    PersonalService.Given(Request.Create().WithUrl(new RegexMatcher("/12345678901")))
      .AtPriority(int.MinValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(request => JsonSerializer.Serialize(new
        {
          education = "None",
          yearsOfWorkExperience = 1,
          occupation = "Other"
        })).WithHeader("Content-Type", MediaTypeNames.Application.Json)
      );
  }

  private void StubMonthlyCostService()
  {
    MonthlyCostService.Given(Request.Create()
        .WithUrl(new RegexMatcher("/[0-9]{11}"))
        .UsingGet())
      .AtPriority(int.MaxValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(message => "800")
        .WithHeader("Content-Type", MediaTypeNames.Application.Json));
    MonthlyCostService.Given(Request.Create()
        .WithUrl(new RegexMatcher("/12345678901"))
        .UsingGet())
      .AtPriority(int.MinValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(message => "100000")
        .WithHeader("Content-Type", MediaTypeNames.Application.Json));
  }

  private void StubIncomeService()
  {
    IncomeService.Given(Request.Create()
        .WithUrl(new RegexMatcher("/[0-9]{11}"))
        .UsingGet())
      .AtPriority(int.MaxValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(message => "500")
        .WithHeader("Content-Type", MediaTypeNames.Application.Json));
    IncomeService.Given(Request.Create()
        .WithUrl(new RegexMatcher("/12345678901"))
        .UsingGet())
      .AtPriority(int.MinValue)
      .RespondWith(Response.Create().WithStatusCode(200)
        .WithBody(message => "0")
        .WithHeader("Content-Type", MediaTypeNames.Application.Json));
  }

}