using System;
using LoanOrders.Frauds;
using LoanOrders.Orders;
using LoanOrders.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Prometheus.SystemMetrics;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

namespace LoanOrders;

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
    services.Configure<MongoDbConfiguration>(Configuration.GetSection(nameof(MongoDbConfiguration)));
    services.AddDiscoveryClient(Configuration);
    services.AddTransient<LoanOrderService>();
    services.AddHttpClient<FraudWebClient>()
      .AddServiceDiscovery();
    services.AddSingleton<LoanOrderRepository>();
    services.AddLogging(builder => builder
      .AddConsole()
      .AddDebug());

    //Dodajemy OpenTracing - tylko do rozproszonego śledzenia, bo metryki
    //w OpenTracing tym momencie są w powijakach...
    services.AddOpenTelemetry()
      .ConfigureResource(
        // Nazwa naszej aplikacji:
        builder => builder.AddService("LoanOrders")
      ).WithTracing(
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

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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