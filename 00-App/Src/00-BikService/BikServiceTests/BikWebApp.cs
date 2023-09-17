using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BikServiceTests;

public class BikWebApp : WebApplicationFactory<Program>
{
  private IHost _host = default!;
  private readonly Action<IServiceCollection> _configureServices;

  public BikWebApp() : this(_ => { }) { }

  public BikWebApp(Action<IServiceCollection> configureServices)
  {
    _configureServices = configureServices;
  }

  protected override IHost CreateHost(IHostBuilder builder)
  {
    _host = base.CreateHost(builder);
    return _host;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    builder.ConfigureServices(_configureServices);
    builder.ConfigureLogging(loggingBuilder =>
    {
      loggingBuilder.ClearProviders();
      loggingBuilder.AddNUnit();
    });
  }

  public IFlurlRequest Request(params object[] urlSegments) => FlurlClient.Request(urlSegments);

  private FlurlClient FlurlClient => new(CreateClient());

  public override async ValueTask DisposeAsync()
  {
    await (_host?.StopAsync() ?? Task.CompletedTask);
    await base.DisposeAsync();
  }
}