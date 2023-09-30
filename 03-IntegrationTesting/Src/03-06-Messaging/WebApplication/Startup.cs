using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Verifier;
using WebApplication.Verifier.Customers;
using WebApplication.Verifier.Customers.Verification;

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
    services.Configure<RabbitMqConfiguration>(Configuration.GetSection(nameof(RabbitMqConfiguration)));
    services.AddSingleton<CustomerVerifier>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddSingleton<FraudInputQueue>();
    services.AddSingleton<IFraudAlertNotifier, MessagingFraudAlertNotifier>();
    services.AddSingleton<IFraudDestination, RabbitMqDestination>();
    services.AddSingleton<IFraudListener, MessagingFraudListener>();
    services.AddEntityFrameworkInMemoryDatabase();
    services.AddDbContext<IVerificationRepository, InMemoryVerificationRepository>(
      // używamy prostej bazy danych w pamięci:
      (context, options) => options.UseInMemoryDatabase("verified")
        .UseInternalServiceProvider(context),
      // Ustawiamy styl życia Transient, bo jeden obiekt DbContext
      // może być używany przez tylko 1 wątek/task,
      // więc upewniamy się, że test dostanie inną instancję
      // niż kod produkcyjny
      ServiceLifetime.Transient);
    services.AddTransient<IReadOnlyCollection<IVerification>>(
      context => new List<IVerification>
      {
        context.GetRequiredService<AgeVerification>(),
        context.GetRequiredService<IdentificationNumberVerification>(),
      });
    services.AddLogging(builder => builder.AddConsole());
    services.Configure<KestrelServerOptions>(options =>
    {
      options.AllowSynchronousIO = true;
    });
    services.AddControllers()
      .AddNewtonsoftJson()
      .AddControllersAsServices();
  }

  public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env,
    FraudInputQueue fraudInputQueue,
    IFraudListener fraudListener)
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
    });

    // Rejestrujemy obserwatora wiadomości z kolejki
    fraudInputQueue.Register(fraudListener);
  }
}