using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Client;
using WebApplication.Controllers;

namespace WebApplication;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  /// <summary>
  /// Korzeń kompozycji frameworku.
  /// </summary>
  /// <param name="services">budowniczy kontenera</param>
  public void ConfigureServices(IServiceCollection services)
  {
    //W Asp.Net Kontrolery domyślnie nie są tworzone przez kontener (tylko przez inny mechanizm),
    //więc trzeba je samemu dodać
    //- albo tutaj (używając AddControllersAsServices), albo w teście.
    services.AddSingleton<ICustomerVerifier, CustomerVerifier>();
    services.AddTransient<FraudController>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IReadOnlyCollection<IVerification>>(
      context => new List<IVerification>
      {
        context.GetRequiredService<AgeVerification>()
      });
    services.AddLogging(builder => builder.AddConsole());
    services.Configure<KestrelServerOptions>(options =>
    {
      options.AllowSynchronousIO = true;
    });
    services.AddControllers()
      .AddNewtonsoftJson()
      .AddControllersAsServices(); //rejestrujemy kontrolery w kontenerze
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
  }
}