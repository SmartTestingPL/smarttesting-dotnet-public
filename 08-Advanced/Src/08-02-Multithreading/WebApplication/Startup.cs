using System.Collections.Generic;
using System.Reactive.Concurrency;
using Core.Verifier.Model;
using Core.Verifier.Model.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplication;

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services)
  {
    AddDependenciesTo(services);
    services.AddControllers().AddNewtonsoftJson();
  }

  public static void AddDependenciesTo(IServiceCollection services)
  {
    services.AddLogging(builder => builder.AddConsole());
    services.AddSingleton<_03_VerificationListener>();
    services.AddTransient<IScheduler>(provider => Scheduler.Default);
    services.AddTransient<_01_CustomerVerifier>();
    services.AddTransient<IEventEmitter, EventEmitter>();
    services.AddTransient<IFraudAlertNotifier, _04_VerificationNotifier>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddTransient<NameVerification>();
    services.AddTransient<IReadOnlyCollection<IVerification>>(context => new List<IVerification>
    {
      context.GetRequiredService<AgeVerification>(),
      context.GetRequiredService<IdentificationNumberVerification>(),
      context.GetRequiredService<NameVerification>(),
    });
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
  }
}