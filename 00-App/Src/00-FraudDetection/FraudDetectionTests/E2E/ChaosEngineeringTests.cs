using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Ductus.FluentDocker.Services;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

// Dotyczy lekcji 05-04
namespace FraudDetectionTests.E2E;

[Category("chaos")]
public class ChaosEngineeringTests : ContainerRunning
{

  private static ICompositeService _container = default!;
  private IContainer _bikService = default!;
  private NUnitConsumer _nUnitConsumer = default!;
  private IContainer? _fraudService;

  [OneTimeSetUp]
  public async Task OneTimeSetUp()
  {
    //Wyjątkowo w przykładzie budujemy najświeższe obrazy obu aplikacji
    await Task.WhenAll(
      BuildBikServiceDockerContainer(),
      BuildFraudDetectionServiceDockerContainer());

    _container = Infrastructure();
    _container.Start();
    PrintRunningServices(_container);
  }

  [OneTimeTearDown]
  public void OneTimeTearDown()
  {
    _container?.Dispose();
  }

  [SetUp]
  public async Task SetUp()
  {
    _nUnitConsumer = new NUnitConsumer();
    _bikService = LatestBikService(_nUnitConsumer);
    await _bikService.StartAsync();
  }

  [TearDown]
  public async Task TearDown()
  {
    _nUnitConsumer.Dispose();
    await (_bikService?.DisposeAsync() ?? new ValueTask(Task.CompletedTask));
  }

  /// <summary>
  /// Hipoteza stanu ustalonego
  ///     POST na URL “/{pesel}”, z peselem osoby nie będącej oszustem, odpowie statusem 403, w ciągu 500 ms
  /// Metoda
  ///     Włączamy błędy spowodowane integracją z komponentami typu @Service
  /// Wycofanie
  ///     Wyłączamy błędy spowodowane integracją z komponentami typu @Service
  /// </summary>
  [Ignore("Wywali się bo nie mamy obsługi błędów na poziomie kontrollerów (wyjątek poleci z serviceu)")]
  [Test]
  public async Task ShouldReturn401Within2000MsWhenCheckingNonFraudButServicesAreDown()
  {
    try
    {
      await StartFraudService(_bikService, _nUnitConsumer);
      await EnableServiceExceptionAssault(_fraudService!);
      await InvalidateCaches(_bikService, _fraudService!);

      var flurlClient = new FlurlClient
      {
        Settings =
        {
          Timeout = 2.Seconds()
        }
      };
      var response = await flurlClient.Request(FraudCheck(_fraudService!))
        .WithHeader(HeaderNames.ContentType, MediaTypeNames.Application.Json)
        .AllowAnyHttpStatus()
        .PostJsonAsync(NonFraud);

      response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }
    finally
    {
      await Cleanup(_fraudService);
    }
  }

  private async Task Cleanup(IContainer? fraudService)
  {
    if (fraudService != null)
    {
      await ClearAssaults(fraudService);
      await fraudService.DisposeAsync();
    }
  }

  protected override IContainer FraudService(
    IContainer bikService,
    IOutputConsumer outputConsumer)
  {
    return _fraudService ??= base.FraudService(bikService, outputConsumer);
  }

  private static readonly object NonFraud = new
  {
    Guid = "5cd495e7-9a66-4c4b-bba2-8d15cc8d9e68",
    Person = new
    {
      Name = "Stefania",
      Surname = "Stefanowska",
      DateOfBirth = "1989-05-01",
      Gender = "Female",
      NationalIdentificationNumber = "89050193724"
    }
  };

  private async Task ClearAssaults(IContainer fraudService)
  {
    await FraudChaos(fraudService).AppendPathSegment("clearAssaults")
      .PostStringAsync(string.Empty);
  }

  private async Task EnableServiceExceptionAssault(IContainer fraudService)
  {
    await FraudChaos(fraudService).AppendPathSegment("enableServiceExceptionAssault")
      .PostStringAsync(string.Empty);
  }

  private async Task InvalidateCaches(params IContainer[] services)
  {
    foreach (var service in services)
    {
      await ("http://localhost:" + service.GetMappedPublicPort(80)).AppendPathSegment("chaos")
        .AppendPathSegment("invalidateCaches")
        .PostStringAsync(string.Empty);
    }
  }

  private string FraudCheck(IContainer fraudService)
  {
    return "http://localhost:"
           + fraudService.GetMappedPublicPort(FraudServiceContainerInternalPort) +
           "/fraud/fraudCheck";
  }

  private IFlurlRequest FraudChaos(IContainer fraudService)
  {
    return new FlurlRequest(
        "http://localhost:" +
        fraudService.GetMappedPublicPort(FraudServiceContainerInternalPort))
      .AppendPathSegment("chaos");
  }
}