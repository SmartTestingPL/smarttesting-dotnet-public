using LoanOrders.Frauds;
using LoanOrders.Orders;
using LoanOrders.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddOptions();
    services.Configure<MongoDbConfiguration>(Configuration.GetSection(nameof(MongoDbConfiguration)));
    services.AddDiscoveryClient(Configuration);
    services.AddTransient<LoanOrderService>();
    services.AddSingleton<LoanOrderRepository>();
    services
      .AddHttpClient<FraudWebClient>()
      .AddServiceDiscovery();
    services.AddLogging(builder => builder
      .AddConsole()
      .AddDebug());
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
    app.UseDiscoveryClient();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
  }
}