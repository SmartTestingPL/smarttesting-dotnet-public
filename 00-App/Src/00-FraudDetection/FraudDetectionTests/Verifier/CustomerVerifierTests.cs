using System;
using System.Collections.Generic;
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
using Testcontainers.PostgreSql;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 03-05
public class CustomerVerifierTests
{
  private static PostgreSqlContainer _container = default!;
  private IHost _app = default!;

  [SetUp]
  public async Task SetUp()
  {
    //Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
    Environment.SetEnvironmentVariable("REAPER_DISABLED", "1");

    _container
      = new PostgreSqlBuilder()
        //Ustawienie wolnego portu. Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
        .WithPortBinding(5432, true)
        .WithDatabase("postgresik")
        .WithImage("postgres:15.4")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _container.StartAsync();

    _app = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseTestServer()
          .UseStartup<Startup>()
          .ConfigureServices(ctx =>
          {
            ctx.AddDbContext<IVerificationRepository, PostgreSqlRepository>();
            ctx.AddSingleton(Substitute.For<IFraudInputQueue>());
          })
          .ConfigureAppConfiguration(configurationBuilder =>
          {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
              ["PostgreSqlOptions:ConnectionString"] = _container.GetConnectionString()
            });
          })).Build();
    await _app.StartAsync();
  }

  [TearDown]
  public async Task TearDown()
  {
    await _app.StopAsync();
    _app.Dispose();
    await _container.DisposeAsync();
  }

  [Test]
  public async Task ShouldSuccessfullyVerifyACustomerWhenPreviouslyVerified()
  {
    await using var scope = _app.Services.CreateAsyncScope();
    await using var repository = scope.ServiceProvider.GetRequiredService<IVerificationRepository>();
    var verifier = scope.ServiceProvider.GetRequiredService<ICustomerVerifier>();

    var zbigniew = Zbigniew();
    repository.FindByUserId(zbigniew.Guid).HasValue.Should().BeTrue();

    var result = await verifier.Verify(zbigniew);

    result.UserId.Should().Be(Zbigniew().Guid);
    result.Status.Should().Be(VerificationStatus.VerificationPassed);
  }

  private static Customer Zbigniew()
  {
    return new Customer(Guid.Parse("89c878e3-38f7-4831-af6c-c3b4a0669022"), YoungZbigniew());
  }

  private static Person YoungZbigniew()
  {
    return new Person("", "", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Male, "18210116954");
  }
}