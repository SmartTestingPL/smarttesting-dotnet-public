using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Ductus.FluentDocker.Services;
using FluentAssertions;
using Flurl.Http;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace FraudDetectionTests.E2E;

/// <summary>
/// Testy end to end, stawiające infrastrukturę oraz obie aplikacje.
/// </summary>
// Dotyczy lekcji 4.1.2
[Category("e2e")]
public class E2ETests : ContainerRunning
{
  private IContainer _bikService = default!;
  private NUnitConsumer _outputConsumer = default!;
  private ICompositeService _infrastructure = default!;

  [OneTimeSetUp]
  public async Task OneTimeSetUp()
  {
    //Wyjątkowo w przykładzie budujemy najświeższe obrazy obu aplikacji
    await Task.WhenAll(
      BuildBikServiceDockerContainer(),
      BuildFraudDetectionServiceDockerContainer());
  }

  [SetUp]
  public async Task SetUp()
  {
    _outputConsumer = new NUnitConsumer();
    
    //Wpierw stawiamy infrastrukturę.
    _infrastructure = Infrastructure();
    _infrastructure.Start();
    PrintRunningServices(_infrastructure);

    //Następnie stawiamy bik-service.
    //Musi być uruchomiony jako pierwszy,
    //ponieważ fraud-detection będzie potrzebował jego URLa.
    _bikService = LatestBikService(_outputConsumer);
    await _bikService.StartAsync();
  }

  [TearDown]
  public async Task TearDown()
  {
    _infrastructure?.Dispose();
    await (_bikService?.StopAsync() ?? Task.CompletedTask);
    _outputConsumer.Dispose();
  }

  /// <summary>
  /// Przykład żądania HTTP dla oszusta.
  /// </summary>
  private static readonly object Fraud = new 
  {
    Guid = "cc8aa8ff-40ff-426f-bc71-5bb7ea644108",
    Person = new {
      Name = "Fraudeusz",
      Surname = "Fraudowski",
      DateOfBirth = "1980-01-01",
      Gender = "Male",
      NationalIdentificationNumber = "00262161334"
    }
  };

  /// <summary>
  /// Przykład żądania HTTP dla osoby nie będącej oszustem.
  /// </summary>
  private static readonly object NonFraud = new
  {
    Guid = "89c878e3-38f7-4831-af6c-c3b4a0669022",
    Person = new
    {
      Name = "Stefania",
      Surname = "Stefanowska",
      DateOfBirth = "1989-05-01",
      Gender = "Female",
      NationalIdentificationNumber = "89050193724"
    }
  };

  [Test]
  [Ignore("Testy nie przejda bo mamy bledy w implementacji")]
  public async Task ShouldReturnProperStatusesForFraudAndNonFraud()
  {
    await using var fraudService = await StartFraudService(_bikService, _outputConsumer);
    var response = await ApplyForLoan(fraudService, Fraud);

    response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);

    response = await ApplyForLoan(fraudService, NonFraud);

    response.StatusCode.Should().Be((int)HttpStatusCode.OK);
  }

  /// <summary>
  /// Aplikuje o pożyczkę.
  /// </summary>
  /// <param name="fraudService">kontener z fraud-service </param>
  /// <param name="data">payload ciało żądania HTTP</param>
  /// <returns>odpowiedź na żądanie HTTP</returns>
  private async Task<IFlurlResponse> ApplyForLoan(IContainer fraudService, object data)
  {
    var endpoint = "http://localhost:" + fraudService.GetMappedPublicPort(FraudServiceContainerInternalPort) + "/fraud/fraudCheck";
    return await endpoint
      .WithHeader(HeaderNames.ContentType, MediaTypeNames.Application.Json)
      .AllowAnyHttpStatus()
      .PostJsonAsync(data);
  }
}