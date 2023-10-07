using System.Collections.Generic;
using FraudVerifier.Customers;
using FraudVerifier.Customers.Verification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;

namespace FraudVerifier;

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
    services.AddDiscoveryClient(Configuration);
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
    app.UseAuthorization();
    app.UseDiscoveryClient();
    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
  }
}