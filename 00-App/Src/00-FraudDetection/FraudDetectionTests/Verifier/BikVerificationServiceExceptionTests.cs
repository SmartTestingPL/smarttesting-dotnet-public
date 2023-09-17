using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Maybe;
using FluentAssertions;
using FraudDetection;
using FraudDetection.Customers;
using FraudDetection.Lib;
using FraudDetection.Verifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 03-04
public class BikVerificationServiceExceptionTests
{
  private WireMockServer _wireMockServer = default!;
  private IBikVerificationService _service = default!;
  private IHost _app = default!;

  [SetUp]
  public async Task SetUp()
  {
    _wireMockServer = WireMockServer.Start();
    _app = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseTestServer()
          .UseStartup<Startup>()
          .ConfigureServices(ctx =>
          {
            ctx.AddSingleton(Substitute.For<IVerificationRepository>());
            ctx.AddSingleton(Substitute.For<IFraudInputQueue>());
          })
          .ConfigureAppConfiguration(configurationBuilder =>
          {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
              ["BikServiceOptions:BaseUrl"] = _wireMockServer.Urls.Single()
            });
          })).Build();
    await _app.StartAsync();
    _service = _app.Services.GetRequiredService<IBikVerificationService>();
  }

  [TearDown]
  public async Task TearDown()
  {
    await _app.StopAsync();
    _app.Dispose();
    _wireMockServer.Stop();
  }

  [Test]
  public async Task ShouldReturnPositiveVerification()
  {
    // Zaślepiamy wywołanie GET, zwracając odpowiednią wartość tekstową
    _wireMockServer.Given(
        Request.Create()
          .WithPath("/18210116954")
          .UsingGet())
      .RespondWith(Response.Create()
        .WithBodyAsJson(new { status = "VerificationPassed" }));

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationPassed);
  }

  [Test]
  public async Task ShouldReturnNegativeVerification()
  {
    // Zaślepiamy wywołanie GET, zwracając odpowiednią wartość tekstową
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithBodyAsJson(new { status = "VerificationFailed" }));

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  // W tym i kolejnych testach zaślepiamy wywołanie GET zwracając różne
  // błędy techniczne. Chcemy się upewnić, że potrafimy je obsłużyć.
  // .NETowy Wiremock nie wspiera wszystkich błędów, które obsługuje
  // wersja Javowa, stąd przykładów jest o kilka mniej.
  [Test]
  public async Task ShouldFailWithMalformedResponseChunk()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithFault(FaultType.MALFORMED_RESPONSE_CHUNK)
    );

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  [Test]
  public async Task ShouldFailWithEmptyResponse()
  {
    _wireMockServer.Given(
      Request.Create()
        .WithPath("/18210116954")
        .UsingGet()).RespondWith(
      Response.Create()
        .WithFault(FaultType.EMPTY_RESPONSE)
    );

    (await _service.Verify(Zbigniew()))
      .Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  private static Customer Zbigniew()
  {
    return new Customer(Guid.NewGuid(), YoungZbigniew());
  }

  private static Person YoungZbigniew()
  {
    return new Person(
      string.Empty,
      string.Empty,
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      "18210116954");
  }
}