using System.Text.Json.Serialization;
using BikService.Analysis;
using BikService.Cost;
using BikService.Credit;
using BikService.Income;
using BikService.Personal;
using BikService.Social;
using Core.Scoring;
using Core.Scoring.Analysis;
using Core.Scoring.Cost;
using Core.Scoring.Credit;
using Core.Scoring.Income;
using Core.Scoring.Personal;
using Core.Scoring.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BikService;

public static class ConfigurationSteps
{
  public static void ReplaceWithDevServices(this IServiceCollection serviceCollection)
  {
    var descriptor = serviceCollection.SingleOrDefault(
      d => d.ServiceType ==
           typeof(DbContextOptions<EfCoreOccupationRepository>));

    if (descriptor != null)
    {
      serviceCollection.Remove(descriptor);
    }

    serviceCollection.Replace(ServiceDescriptor.Singleton<ICostRestTemplateClient, DevCostRestTemplateClient>());
    serviceCollection.Replace(ServiceDescriptor.Singleton<IIncomeRestTemplateClient, DevIncomeRestTemplateClient>());
    serviceCollection.Replace(ServiceDescriptor.Singleton<IPersonalRestTemplateClient, DevPersonalRestTemplateClient>());
    serviceCollection.Replace(ServiceDescriptor.Singleton<ISocialRestTemplateClient, DevSocialRestTemplateClient>());
    serviceCollection.AddScoped<DevOccupationRepository>();
    serviceCollection.Replace(ServiceDescriptor.Scoped<IOccupationRepository, DevOccupationRepository>());
    serviceCollection.Replace(ServiceDescriptor.Scoped<IOccupationRepositoryInitialization>(ctx => new DevOccupationRepositoryInitialization()));
    serviceCollection.Replace(ServiceDescriptor.Singleton<IScoreUpdater, DevScoreUpdater>());
    serviceCollection.AddSingleton<DevCreditInfoRepository>();
    serviceCollection.Replace(ServiceDescriptor.Singleton<ICreditInfoRepository>(ctx => ctx.GetRequiredService<DevCreditInfoRepository>()));
    serviceCollection.Replace(ServiceDescriptor.Singleton<ICreditInfoRepositoryForInitialization>(ctx => ctx.GetRequiredService<DevCreditInfoRepository>()));
    serviceCollection.AddSingleton<ICreditQueueInitialization, DevCreditQueueInitialization>();
  }

  public static void AddAppServices(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddMemoryCache();
    serviceCollection.AddDbContext<EfCoreOccupationRepository>(optionsBuilder =>
      optionsBuilder
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
    );
    serviceCollection.AddSingleton<IExecutor, TaskBasedExecutor>();
    serviceCollection.AddScoped<OccupationRepositoryWatcher>();
    serviceCollection.AddScoped<CachedOccupationRepository>();
    serviceCollection.AddScoped<IOccupationRepository>(ctx =>
      ctx.GetRequiredService<CachedOccupationRepository>());
    serviceCollection.AddScoped<IScoreAnalyzer>(ctx =>
      new ScoreAnalyzer(
        ctx.GetRequiredService<ICompositeScoreEvaluation>(),
        ctx.GetRequiredService<IOptions<ScoreOptions>>().Value.Threshold,
        ctx.GetRequiredService<ILogger<ScoreAnalyzer>>()));
    serviceCollection.AddScoped<ICompositeScoreEvaluation, ParallelCompositeScoreEvaluation>();
    serviceCollection.AddScoped(ctx => new List<IScoreEvaluation>
    {
      ctx.GetRequiredService<CreditInfoScoreEvaluation>(),
      ctx.GetRequiredService<MonthlyCostScoreEvaluation>(),
      ctx.GetRequiredService<MonthlyIncomeScoreEvaluation>(),
      ctx.GetRequiredService<PersonalInformationScoreEvaluation>(),
      ctx.GetRequiredService<SocialStatusScoreEvaluation>()
    });
    serviceCollection.AddScoped<CreditInfoScoreEvaluation>();
    serviceCollection.AddScoped<MonthlyCostScoreEvaluation>();
    serviceCollection.AddScoped<MonthlyIncomeScoreEvaluation>();
    serviceCollection.AddScoped<PersonalInformationScoreEvaluation>();
    serviceCollection.AddScoped<SocialStatusScoreEvaluation>();

    serviceCollection.AddSingleton<IScoreUpdater, RabbitCreditScoreUpdater>();
    serviceCollection.AddSingleton(ctx =>
    {
      var options = ctx.GetRequiredService<IOptions<CreditDbOptions>>();
      return new MongoTemplate<CreditInfoDocument>(
        options.Value.ConnectionString,
        options.Value.DatabaseName);
    });
    serviceCollection.AddSingleton<MongoTemplateCreditInfoRepository>();
    serviceCollection.AddSingleton<CreditInfoRepositoryWatcher>();
    serviceCollection.AddSingleton<CachedCreditInfoRepository>();
    serviceCollection.AddSingleton<ICreditInfoRepositoryForInitialization>(ctx => ctx.GetRequiredService<CachedCreditInfoRepository>());
    serviceCollection.AddSingleton<ICreditInfoRepository>(ctx => ctx.GetRequiredService<CachedCreditInfoRepository>());
    serviceCollection.AddSingleton<IMonthlyIncomeClient>(ctx =>
      new MonthlyIncomeClient(
        ctx.GetRequiredService<IIncomeRestTemplateClient>(),
        ctx.GetRequiredService<IOptions<IncomeServiceOptions>>().Value.BaseUrl,
        ctx.GetRequiredService<ILogger<MonthlyIncomeClient>>()));
    serviceCollection.AddSingleton<IMonthlyCostClient>(ctx =>
      new MonthlyCostClient(
        ctx.GetRequiredService<ICostRestTemplateClient>(),
        ctx.GetRequiredService<IOptions<MonthlyCostServiceOptions>>().Value.BaseUrl,
        ctx.GetRequiredService<ILogger<MonthlyCostClient>>()
      ));
    serviceCollection.AddSingleton<IPersonalInformationClient, PersonalInformationClient>(ctx =>
      new PersonalInformationClient(
        ctx.GetRequiredService<IPersonalRestTemplateClient>(),
        ctx.GetRequiredService<IOptions<PersonalServiceOptions>>().Value.BaseUrl,
        ctx.GetRequiredService<ILogger<PersonalInformationClient>>()));
    serviceCollection.AddSingleton<ISocialStatusClient, SocialStatusClient>(ctx =>
      new SocialStatusClient(
        ctx.GetRequiredService<ISocialRestTemplateClient>(),
        ctx.GetRequiredService<IOptions<SocialServiceOptions>>().Value.BaseUrl,
        ctx.GetRequiredService<ILogger<SocialStatusClient>>()));
    serviceCollection.AddSingleton<ICostRestTemplateClient, CostRestTemplateClient>();
    serviceCollection.AddSingleton<IIncomeRestTemplateClient, IncomeRestTemplateClient>();
    serviceCollection.AddSingleton<IPersonalRestTemplateClient, PersonalRestTemplateClient>();
    serviceCollection.AddSingleton<ISocialRestTemplateClient, SocialRestTemplateClient>();
    serviceCollection.AddSingleton<RestTemplate>();
    serviceCollection.AddSingleton<ICreditInputQueue, CreditInputRabbitMqQueue>();
    serviceCollection.AddSingleton<RabbitCreditInfoListener>();
    serviceCollection.AddSingleton<IOccupationRepositoryInitialization, PostgreSqlOccupationRepositoryInitialization>();
    serviceCollection.AddSingleton<IMongoDbInitialization, MongoDbInitialization>();
    serviceCollection.AddSingleton<ICreditQueueInitialization, RabbitMqCreditQueueInitialization>();
    serviceCollection.AddHttpClient();

    serviceCollection.AddControllers().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    serviceCollection.AddEndpointsApiExplorer();
    serviceCollection.AddSwaggerGen();
  }

  public static async Task AddInitialPostgresData(this IServiceProvider serviceProvider)
  {
    await serviceProvider.GetRequiredService<IOccupationRepositoryInitialization>().Perform(serviceProvider);
  }

  public static async Task AddInitialMongoData(this IServiceProvider serviceProvider)
  {
    await serviceProvider.GetRequiredService<IMongoDbInitialization>().Perform(serviceProvider);
  }

  public static void RegisterQueueListener(this IServiceProvider serviceProvider)
  {
    serviceProvider.GetRequiredService<ICreditQueueInitialization>().Perform(serviceProvider);
  }
}

public class DevCreditQueueInitialization : ICreditQueueInitialization
{
  public void Perform(IServiceProvider appServices)
  {
    
  }
}

public class DevOccupationRepositoryInitialization : IOccupationRepositoryInitialization
{
  public async Task Perform(IServiceProvider serviceProvider)
  {
    await Task.CompletedTask;
  }
}