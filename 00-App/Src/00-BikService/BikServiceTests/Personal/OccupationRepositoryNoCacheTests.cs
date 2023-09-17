using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BikService;
using BikService.Personal;
using Core.Scoring.domain;
using Core.Scoring.Personal;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace BikServiceTests.Personal;

// TODO: Dotyczy lekcji 03-05
public class OccupationRepositoryNoCacheTests
{
  private PostgreSqlContainer _postgreSql = default!;
  private ServiceProvider _container = default!;

  [SetUp]
  public async Task SetUp()
  {
    _postgreSql
      = new PostgreSqlBuilder()
        //Ustawienie wolnego portu. Obejście na https://github.com/isen-ng/testcontainers-dotnet/issues/58
        .WithPortBinding(5432, true)
        .WithDatabase(Guid.NewGuid().ToString("N"))
        .WithImage("postgres:15.4")
        .WithUsername("admin")
        .WithPassword("nimda")
        .Build();
    await _postgreSql.StartAsync();
    _container = await CreateServiceProvider(_postgreSql.GetConnectionString());
  }

  [Test]
  public void ShouldRetrieveOccupationScoresFromCache()
  {
    var occupationScores = GetOccupationScores();

    var expectedValues = Enum.GetValues<PersonalInformation.Occupations>();
    occupationScores.Should().ContainKeys(
      expectedValues.Cast<PersonalInformation.Occupations?>());
  }

  private Dictionary<PersonalInformation.Occupations?, Score> GetOccupationScores()
  {
    using var scope = _container.CreateAsyncScope();
    var repository = scope.ServiceProvider.GetRequiredService<EfCoreOccupationRepository>();
    return repository.GetOccupationScores();
  }

  private async Task<ServiceProvider> CreateServiceProvider(string connectionString)
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddAppServices();
    serviceCollection.AddSingleton(Options.Create(new OccupationRepositoryOptions
    {
      ConnectionString = connectionString
    }));
    serviceCollection.AddLogging(builder => builder.AddNUnit());
    var serviceProvider = serviceCollection.BuildServiceProvider();
    await serviceProvider.AddInitialPostgresData();
    return serviceProvider;
  }
}
