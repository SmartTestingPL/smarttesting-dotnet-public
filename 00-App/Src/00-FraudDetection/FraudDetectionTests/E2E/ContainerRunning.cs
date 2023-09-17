using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Humanizer;
using NUnit.Framework;
using static AtmaFileSystem.AtmaFileSystemPaths;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;
using ContainerBuilder = DotNet.Testcontainers.Builders.ContainerBuilder;

namespace FraudDetectionTests.E2E;

public class ContainerRunning
{
  protected const string ExternalHostname = "host.docker.internal";
  private const int BikContainerInternalPort = 80;
  protected const int FraudServiceContainerInternalPort = 80;

  /// <param name="outputConsumer"></param>
  /// <returns>kontener z najnowszą wersją bik-service</returns>
  protected static IContainer LatestBikService(IOutputConsumer outputConsumer)
  {
    return new ContainerBuilder()
      .WithImage("bikservice:latest")
      .WithPortBinding(BikContainerInternalPort, true)
      .WithEnvironment("SocialService__BaseUrl", $"http://{ExternalHostname}:4567")
      .WithEnvironment("PersonalService__BaseUrl", $"http://{ExternalHostname}:2345")
      .WithEnvironment("MonthlyCostService__BaseUrl", $"http://{ExternalHostname}:3456")
      .WithEnvironment("IncomeService__BaseUrl", $"http://{ExternalHostname}:1234")
      .WithEnvironment("RabbitMq__ConnectionString", $"amqp://guest:guest@{ExternalHostname}:5672")
      .WithEnvironment("OccupationRepository__ConnectionString",
        $"Server={ExternalHostname};Port=5432;Database=test;User Id=test;Password=test;")
      .WithEnvironment("CreditDb__ConnectionString", $"mongodb://root:example@{ExternalHostname}:27017")
      .WithEnvironment("Zipkin__Host", ExternalHostname)
      .WithCleanUp(true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(BikContainerInternalPort))
      .WithOutputConsumer(outputConsumer)
      .Build();
  }

  /// <returns>kontenery z uruchomioną infrastrukturą</returns>
  protected static ICompositeService Infrastructure()
  {
    return new Builder()
      .UseContainer()
      .UseCompose()
      .FromFile(
        AbsoluteDirectoryPath.OfThisFile()
          .ParentDirectory().Value()
          .AddDirectoryName("Docker")
          .AddFileName("docker-compose-e2e.yml")
          .ToString())
      .WaitForPort("zipkin_1", "9411/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("postgres-bik_1", "5432/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("postgres-fraud_1", "5433/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("rabbitmq_1", "5672/tcp",  (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("mongo_1", "27017/tcp",  (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("income-wiremock_1", "1234/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("personal-wiremock_1", "2345/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("monthly-cost-wiremock_1", "3456/tcp", (long)60.Seconds().TotalMilliseconds)
      .WaitForPort("social-wiremock_1", "4567/tcp", (long)60.Seconds().TotalMilliseconds)
      .RemoveOrphans()
      .Build();
  }

  protected static void PrintRunningServices(ICompositeService svc)
  {
    TestContext.Out.WriteLine("Running services: " +
                              string.Join(", ",
                                svc.Services.Select(s => $"{s.Name}={s.State}")));
  }

  protected static async Task BuildBikServiceDockerContainer()
  {
    await Task.Run(() =>
    {
      var dockerfileSlnPath =
        AbsoluteDirectoryPath.OfThisFile().ParentDirectory(2).Value() +
        DirectoryName("00-BikService");
      var dockerFileLocation =
        dockerfileSlnPath +
        DirectoryName("BikService") +
        FileName("Dockerfile");

      using var image = new Builder()
        .DefineImage("bikservice")
        .FromFile(dockerFileLocation.ToString())
        .WorkingFolder(dockerfileSlnPath.ToString())
        .Build();
    });
  }

  protected static async Task BuildFraudDetectionServiceDockerContainer()
  {
    await Task.Run(() =>
    {
      var dockerfileSlnPath = 
        AbsoluteDirectoryPath.OfThisFile().ParentDirectory(1).Value();
      var dockerFileLocation = 
        dockerfileSlnPath + 
        DirectoryName("FraudDetection") + 
        FileName("Dockerfile");

      using var image = new Builder()
        .DefineImage("frauddetection")
        .FromFile(dockerFileLocation.ToString())
        .WorkingFolder(dockerfileSlnPath.ToString())
        .Build();
    });
  }


  /// <param name="bikService">kontener bik-service</param>
  /// <param name="outputConsumer"></param>
  /// <returns>uruchomiony kontener fraud-service</returns>
  protected async Task<IContainer> StartFraudService(IContainer bikService,
    IOutputConsumer outputConsumer)
  {
    var container = FraudService(bikService, outputConsumer);
    await container.StartAsync();
    return container;
  }

  protected virtual IContainer FraudService(
    IContainer bikService,
    IOutputConsumer outputConsumer)
  {
    return new ContainerBuilder()
      .WithImage("frauddetection:latest")
      .WithPortBinding(FraudServiceContainerInternalPort, true)
      .WithEnvironment("BikServiceOptions__BaseUrl", $"http://{ExternalHostname}:{bikService.GetMappedPublicPort(FraudServiceContainerInternalPort)}")
      .WithEnvironment("RabbitMqOptions__ConnectionString", $"amqp://guest:guest@{ExternalHostname}:5672")
      .WithEnvironment("PostgreSqlOptions__ConnectionString", $"Server={ExternalHostname};Port=5433;Database=test;User Id=test;Password=test;")
      .WithEnvironment("Zipkin__Host", ExternalHostname)
      .WithCleanUp(true)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(FraudServiceContainerInternalPort))
      .WithOutputConsumer(outputConsumer)
      .Build();
  }
}