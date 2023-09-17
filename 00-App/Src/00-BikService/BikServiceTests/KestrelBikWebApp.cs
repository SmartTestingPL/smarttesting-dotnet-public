using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BikServiceTests;

/// <summary>
/// Klasa napisana wg //https://github.com/martincostello/dotnet-minimal-api-integration-testing/blob/main/tests/TodoApp.Tests/HttpServerFixture.cs
/// Odpala apkę w Kestrelu, nie w TestServerze, bo Pact.Net (póki co) wymaga prawdziwej końcówki HTTP
/// </summary>
public class KestrelBikWebApp : WebApplicationFactory<Program>
{
  private IHost? _host;
  private readonly Action<IServiceCollection> _configureServices;

  public KestrelBikWebApp(Action<IServiceCollection> configureServices)
  {
    _configureServices = configureServices;
  }

  protected override IHost CreateHost(IHostBuilder builder)
  {
    var testHost = builder.Build();
    builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
    _host = builder.Build();
    _host.Start();
    var server = _host.Services.GetRequiredService<IServer>();
    var addresses = server.Features.Get<IServerAddressesFeature>();

    ClientOptions.BaseAddress = addresses!.Addresses
      .Select(x => new Uri(x))
      .Last();
    testHost.Start();
    return testHost;
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
    builder.ConfigureKestrel(options => { });
    builder.UseUrls("http://[::1]:0");
  }

  public string ServerAddress 
  {
    get 
    {
      EnsureServer();
      return ClientOptions.BaseAddress.ToString();
    }
  }

  public override IServiceProvider Services 
  {
    get 
    {
      EnsureServer();
      return _host!.Services!;
    }
  }

  private void EnsureServer()
  {
    if (_host is null)
    {
      using var _ = CreateDefaultClient();
    }
  }

  public override async ValueTask DisposeAsync()
  {
    await (_host?.StopAsync() ?? Task.CompletedTask);
    await base.DisposeAsync();
  }
}