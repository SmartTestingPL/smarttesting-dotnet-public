﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Verifier;
using WebApplication.Verifier.Verification;

namespace WebApplication;

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
      new BikVerificationService("http://example.com",
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
  public void ConfigureDevelopmentServices(IServiceCollection services)
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
  private void ConfigureWithoutExternalServices(IServiceCollection services)
  {
    services.Configure<RabbitMqConfiguration>(Configuration.GetSection(nameof(RabbitMqConfiguration)));
    services.Configure<PostgreSqlConfiguration>(Configuration.GetSection(nameof(PostgreSqlConfiguration)));
    services.AddScoped<CustomerVerifier>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddTransient<IReadOnlyCollection<IVerification>>(
      context => new List<IVerification>
      {
        context.GetRequiredService<AgeVerification>(),
        context.GetRequiredService<IdentificationNumberVerification>(),
      });
    services.AddLogging(builder => builder.AddConsole().AddDebug());
    services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
    services.AddHealthChecks();
    services.AddControllers()
      .AddNewtonsoftJson()
      .AddControllersAsServices();
  }

  public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env,
    IFraudInputQueue fraudInputQueue,
    IFraudListener fraudListener,
    IVerificationRepository verificationRepository)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapHealthChecks("/health");
    });

    verificationRepository.EnsureExists();
    fraudInputQueue.Register(fraudListener);
  }
}