using System;
using System.Collections.Generic;
using FraudVerifier.Customers;
using FraudVerifier.Customers.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using Steeltoe.Discovery.Client;
using Unleash;
using Unleash.ClientFactory;
using OpenTelemetry.Trace;
using Prometheus;
using Prometheus.SystemMetrics;

namespace FraudVerifier;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddOptions();
    services.Configure<UnleashConfiguration>(Configuration.GetSection(nameof(UnleashConfiguration)));
    // Rejestrujemy klienta Eureki:
    services.AddDiscoveryClient(Configuration);
    // Rejestrujemy klienta Unleash:
    services.AddSingleton(context =>
    {
      var config = context.GetRequiredService<IOptions<UnleashConfiguration>>();
      var unleashFactory = new UnleashClientFactory();
      var unleash = unleashFactory.CreateClient(new UnleashSettings
        {
          AppName = config.Value.AppName,
          UnleashApi = config.Value.UnleashApi,
          CustomHttpHeaders = new Dictionary<string, string>()
          {
            {"Authorization","default:development.unleash-insecure-api-token" }
          }
        },
        //na potrzeby tego ćwiczenia włączamy synchroniczne ściąganie
        //wartości przełączników do pamięci podręcznej, żeby
        //od razu po wystartowaniu usługi mieć wszystkie wartości.
        //Domyślnie klient "dociąga" sobie wartości przełączników w tle.
        true);

      var logger = context.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();
      unleash.ConfigureEvents(cfg =>
      {
        cfg.ImpressionEvent = evt => { logger.LogInformation($"{evt.FeatureName}: {evt.Enabled}"); };
        cfg.ErrorEvent = evt => { logger.LogError($"Error {evt.Error} of type {evt.ErrorType} occured. Resource: {evt.Resource}, Status code: {evt.StatusCode}"); };
      });

      return unleash;
    });
    services.AddTransient<CustomerVerifier>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<NameVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddTransient<IReadOnlyCollection<IVerification>>(
      context => new List<IVerification>
      {
        context.GetRequiredService<AgeVerification>(),
        context.GetRequiredService<NameVerification>(),
        context.GetRequiredService<IdentificationNumberVerification>(),
      });

    // Dodajemy OpenTracing - tylko do rozproszonego śledzenia, bo metryki
    // w OpenTracing tym momencie są w powijakach...
    services.AddOpenTelemetry()
      // Nazwa naszej aplikacji:
      .ConfigureResource(builder => builder.AddService("FraudVerifier"))
      .WithTracing(
        builder =>
        {
          builder
            // Włączamy obsługę śledzenia dla ASP.Net Core
            .AddAspNetCoreInstrumentation()
            // Włączamy obsługę śledzenia HttpClienta, czyli żądań HTTP.
            // Ignorujemy żądania do usług takich jak Eureka czy Unleash.
            .AddHttpClientInstrumentation(options =>
              options.Filter = context => context.RequestUri.Port != 8761 &&
                                          context.RequestUri.Port != 4242)
            // Dodajemy eksport danych śledzenia do Zipkina:
            .AddZipkinExporter(options =>
            {
              // Adres instancji Zipkina:
              options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            })
            // Bonusowo dodajemy eksporter na konsolę, żeby można było
            // sobie też spojrzeć jak to wygląda w logach.
            .AddConsoleExporter();

        });

    // Dodajemy raportowanie do Prometheusa metryk systemowych (np. CPU).
    // Niektóre z tych metryk będą dostępne tylko gdy aplikacja zostanie
    // uruchomiona pod Linuksem
    // (zobacz https://github.com/Daniel15/prometheus-net.SystemMetrics)
    services.AddSystemMetrics();

    services.AddControllers().AddNewtonsoftJson();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseRouting();
      
    // Konfiguracja Prometheusa: włączamy zbieranie metryk
    // z żądań HTTP w ASP.Net Core:
    app.UseHttpMetrics();
    app.UseAuthorization();
    // Włączamy wsparcie dla Eureki:
    app.UseDiscoveryClient();
    app.UseEndpoints(endpoints =>
    {
      // Udostępniamy specjalną końcówką HTTP z metrykami,
      // którą będzie odpytywał Prometheus:
      endpoints.MapMetrics();

      endpoints.MapControllers();
    });
  }
}