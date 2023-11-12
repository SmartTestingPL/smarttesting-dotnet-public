﻿using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using WebApplication;
using WebApplication.Controllers;
using WebApplication.Logic;

namespace WebApplicationTests.Verifier.Tdd;

public class _03_AcceptanceControllerSomethingTests
{
  [Test]
  [Ignore("homework")]
  public async Task ShouldVerifyAClientWithDebtAsFraud()
  {
    using var host = TestHost();
    await host.StartAsync();
    using var flurlClient = FlurlClientFor(host);
    var fraud = ClientWithDebt();
      
    var verification = await VerifyClient(flurlClient, fraud);

    ThenIsVerifiedAsFraud(verification);
  }

  private static async Task<VerificationResult> VerifyClient(
    IFlurlClient flurlClient, 
    Client client)
  {
    return await flurlClient.Request("fraudSomething/fraudCheck")
      .WithHeader("Content-Type", "application/json")
      .PostJsonAsync(client)
      .ReceiveJson<VerificationResult>();
  }

  private static void ThenIsVerifiedAsFraud(VerificationResult verification)
  {
    verification.Status.Should().Be(VerificationStatus.Fraud);
  }

  private static Client ClientWithDebt()
  {
    return new Client(true);
  }

  private static FlurlClient FlurlClientFor(IHost host)
  {
    return new FlurlClient(host.GetTestClient());
  }

  private static IHost TestHost()
  {
    return Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          .UseTestServer()
          .ConfigureTestServices(services => 
            services.AddTransient<Something>()))
      .Build();
  }
}