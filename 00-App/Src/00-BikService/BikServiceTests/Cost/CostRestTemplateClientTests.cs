using System.Linq;
using System.Threading.Tasks;
using BikService;
using BikService.Cost;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Response = WireMock.ResponseBuilders.Response;

namespace BikServiceTests.Cost;

// TODO: Dotyczy lekcji 03-04
public class CostRestTemplateClientTests
{

  private WireMockServer _wireMockServer = default!;
  private ServiceProvider _container = default!;
  private ICostRestTemplateClient Service => _container.GetRequiredService<ICostRestTemplateClient>();

  [SetUp]
  public void SetUp()
  {
    _wireMockServer = WireMockServer.Start();
    _container = GetServiceProvider();
  }

  [TearDown]
  public async Task TearDown()
  {
    _wireMockServer.Stop();
    await _container.DisposeAsync();
  }

  [Test]
  public async Task ShouldFailWithTimeout()
  {
    _wireMockServer.Given(Request.Create().WithUrl(new RegexMatcher("/")).UsingGet())
      .RespondWith(Response.Create().WithDelay(10.Seconds()));

    await Service.Awaiting(s => s.Get<string>($"{_wireMockServer.Urls.First()}/"))
      .Should().ThrowAsync<FlurlHttpTimeoutException>();
  }

  [Test]
  public async Task ShouldFailWithMalformed()
  {
    _wireMockServer.Given(Request.Create().WithUrl(new RegexMatcher("/")).UsingGet())
      .RespondWith(Response.Create().WithFault(FaultType.MALFORMED_RESPONSE_CHUNK));

    await Service.Awaiting(s => s.Get<Empty>(_wireMockServer.Urls.First()))
      .Should().ThrowAsync<FlurlParsingException>();
  }

  private static ServiceProvider GetServiceProvider()
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddAppServices();
    return serviceCollection.BuildServiceProvider();
  }

  public record Empty(string Value);
}

