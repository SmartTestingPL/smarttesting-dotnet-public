using BikService;
using BikService.Cost;
using BikService.Credit;
using BikService.Income;
using BikService.Personal;
using BikService.Social;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOptions();
builder.Services.Configure<SocialServiceOptions>(
  builder.Configuration.GetSection("SocialService"));
builder.Services.Configure<PersonalServiceOptions>(
  builder.Configuration.GetSection("PersonalService"));
builder.Services.Configure<MonthlyCostServiceOptions>(
  builder.Configuration.GetSection("MonthlyCostService"));
builder.Services.Configure<IncomeServiceOptions>(
  builder.Configuration.GetSection("IncomeService"));
builder.Services.Configure<RabbitMqOptions>(
  builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<OccupationRepositoryOptions>(
  builder.Configuration.GetSection("OccupationRepository"));
builder.Services.Configure<CreditDbOptions>(
  builder.Configuration.GetSection("CreditDb"));
builder.Services.Configure<ScoreOptions>(
  builder.Configuration.GetSection("Score"));

builder.Services.AddAppServices();

builder.Host.UseSerilog((context, configuration) => configuration
  .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                   "{Properties:j}{NewLine}{Exception}")
  .Enrich.FromLogContext());

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddOpenTelemetry()
  .ConfigureResource(b => b.AddService("BikService"))
  .WithTracing(
    tracerProviderBuilder =>
    {
      tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddZipkinExporter(options =>
        {
          options.Endpoint = new Uri(
            $"http://{builder.Configuration.GetSection("Zipkin")["Host"]}:9411/api/v2/spans");
        })
        .AddConsoleExporter();
    });
builder.Services.AddSystemMetrics();
builder.Services.AddHealthChecks();

if (builder.Environment.EnvironmentName == "LocalDev")
{
  builder.Services.ReplaceWithDevServices();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "LocalDev")
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHttpMetrics();
app.MapMetrics();
app.MapControllers();
app.MapHealthChecks("/health");

await app.Services.AddInitialPostgresData();
await app.Services.AddInitialMongoData();
app.Services.RegisterQueueListener();

app.Run();


public partial class Program
{
}