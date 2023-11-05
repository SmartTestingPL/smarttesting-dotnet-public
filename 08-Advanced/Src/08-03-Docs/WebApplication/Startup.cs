using System.Collections.Generic;
using Core.Verifier.Model;
using Core.Verifier.Model.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    services.AddSingleton<IEventEmitter, FakeEventEmitter>();
    services.AddTransient<ICustomerVerifier, CustomerVerifier>();
    services.AddTransient<AgeVerification>();
    services.AddTransient<IdentificationNumberVerification>();
    services.AddTransient<NameVerification>();
    services.AddTransient<IReadOnlyCollection<IVerification>>(context =>
      new List<IVerification>
      {
        context.GetRequiredService<AgeVerification>(),
        context.GetRequiredService<IdentificationNumberVerification>(),
        context.GetRequiredService<NameVerification>(),
      });
    services.
      AddControllers()
      .AddControllersAsServices()
      .AddNewtonsoftJson();
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