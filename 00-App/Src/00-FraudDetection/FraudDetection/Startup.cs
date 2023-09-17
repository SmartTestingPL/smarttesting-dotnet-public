using System;
using System.Collections.Generic;
using FraudDetection.Verifier;
using FraudDetection.Verifier.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Prometheus.SystemMetrics;

namespace FraudDetection;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddTransient<IFraudListener, MessagingFraudListener>();
    services.AddTransient<IFraudDestination, RabbitMqDestination>();
    services.AddSingleton<IFraudAlertNotifier, MessagingFraudAlertNotifier>();
    services.AddSingleton<IFraudInputQueue, FraudInputQueue>();
    services.AddSingleton<IBikVerificationService>(context =>
      new BikVerificationService(context.GetRequiredService<IOptions<BikServiceOptions>>(),
        context.GetRequiredService<ILogger<BikVerificationService>>()));
    services.AddDbContext<IVerificationRepository, PostgreSqlRepository>();
    ConfigureWithoutExternalServices(services);
  }

  /// <summary>
  /// Konfiguracja deweloperska, aktywowana, gdy podano profil
  /// Development (wg konwencji nazewniczej:
  /// Configure[ASPNET_ENVIRONMENT]Services).
  /// 
  /// W tym schemacie konfiguracyjnym połączenie do BIK jest zaślepione,
  /// podmienione zostają wszystkie komponenty, które biorą udział
  /// w integracji z usługami zewnętrznymi. 
  /// </summary>
  public void ConfigureLocalDevServices(IServiceCollection services)
  {
    services.AddTransient<IFraudListener>(context =>
      new DevFraudListener(
        new MessagingFraudListener(
          context.GetRequiredService<IVerificationRepository>(),
          context.GetRequiredService<ILogger<MessagingFraudListener>>())));
    services.AddTransient<IFraudAlertNotifier, DevFraudAlertNotifier>();
    services.AddSingleton<IFraudInputQueue, DevFraudInputQueue>();
    services.AddSingleton<IBikVerificationService, DevBikVerificationService>();
    services.AddEntityFrameworkInMemoryDatabase();
    services.AddDbContext<IVerificationRepository, InMemoryVerificationRepository>(
      (context, builder) => builder.UseInMemoryDatabase("verified")
        .UseInternalServiceProvider(context));
    ConfigureWithoutExternalServices(services);
  }

  /// <summary>
  /// Metoda konfigurująca fragment grafu zależności
  /// wspólny dla środowiska produkcyjnego i deweloperskiego.
  /// </summary>
  public void ConfigureWithoutExternalServices(IServiceCollection services)
  {
    services.AddMemoryCache();
    services.Configure<RabbitMqOptions>(Configuration.GetSection(nameof(RabbitMqOptions)));
    services.Configure<PostgreSqlOptions>(Configuration.GetSection(nameof(PostgreSqlOptions)));
    services.Configure<BikServiceOptions>(Configuration.GetSection(nameof(BikServiceOptions)));
    services.AddScoped<CustomerVerifier>();
    services.AddScoped<ICustomerVerifier>(
      ctx => new CustomerVerifierWatcher(ctx.GetRequiredService<CustomerVerifier>()));
    services.AddTransient<GenderVerification>();
    services.AddTransient<NameVerification>();
    services.AddTransient<SurnameVerification>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddTransient<IObjectProvider<IReadOnlySet<IVerification>>>(
      ctx => new ServiceProviderBasedProvider<IReadOnlySet<IVerification>>(ctx));
    services.AddTransient<IReadOnlySet<IVerification>>(
      context => new HashSet<IVerification>
      {
        context.GetRequiredService<AgeVerification>(),
        context.GetRequiredService<GenderVerification>(),
        context.GetRequiredService<NameVerification>(),
        context.GetRequiredService<SurnameVerification>(),
        context.GetRequiredService<IdentificationNumberVerification>()
      });
    services.AddLogging(builder => builder.AddConsole().AddDebug());
    services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
    services.AddHealthChecks();
    services.AddSwaggerGen();

    services.AddOpenTelemetry()
      .ConfigureResource(builder => builder.AddService("FraudDetection"))
      .WithTracing(
        builder =>
        {
          builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddZipkinExporter(options =>
            {
              options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            })
            .AddConsoleExporter();

        });
    services.AddSystemMetrics();

    services.AddControllers()
      .AddNewtonsoftJson()
      .AddControllersAsServices();
  }

  public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env)
  {
    if (env.IsDevelopment() || env.EnvironmentName == "LocalDev")
    {
      app.UseSwagger();
      app.UseSwaggerUI(config =>
      {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "FraudDetection");
      });
      app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapMetrics();
      endpoints.MapHealthChecks("/health");
    });

    using (var scope = app.ApplicationServices.CreateScope())
    {
      var verificationRepository = scope.ServiceProvider.GetRequiredService<IVerificationRepository>();
      verificationRepository.EnsureExists();
    }

    var fraudInputQueue = app.ApplicationServices.GetRequiredService<IFraudInputQueue>();
    var fraudListener = app.ApplicationServices.GetRequiredService<IFraudListener>();
    fraudInputQueue.Register(fraudListener);
  }
}