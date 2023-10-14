using System.Collections.Generic;
using Core.Verifier;
using Core.Verifier.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Verifier.Infrastructure;
using WebApplication.Verifier.Model;

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
    services.AddDbContext<PostgreSqlRepository>();
      
    //opakowanie istniejącego repozytorium dekoratorem.
    services.AddTransient<IVerificationRepository>(context =>
      new VerificationRepositoryWatcher(
        context.GetRequiredService<PostgreSqlRepository>()));

    ConfigureWithoutExternalServices(services);
  }

  /// <summary>
  /// Konwencja: Configure[ASPNET_ENVIRONMENT]Services
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
    services.AddDbContext<IVerificationRepository, InMemoryVerificationRepository>(
      builder => builder.UseInMemoryDatabase("verified"));
    ConfigureWithoutExternalServices(services);
  }

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

    // Rejestrujemy middleware wstrzykujący opóźnienia do kontrolerów,
    // ale pomijamy przy tym kontroler sterujący wstrzykiwaniem!
    app.UseWhen(
      context => !context.Request.Path.StartsWithSegments("/chaos"),
      builder => builder.UseMiddleware<ChaosMiddleware>());

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });

    verificationRepository.EnsureExists();
    fraudInputQueue.Register(fraudListener);
  }
}