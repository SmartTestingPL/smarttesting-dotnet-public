using System.Threading.Tasks;
using LoanOrders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace LoanOrdersTests;

/// <summary>
/// Test weryfikuje czy podnosi się kontekst aplikacyjny.
/// </summary>
public class LoanOrdersApplicationTests
{
  [Test]
  public async Task ShouldLoad()
  {
    using var app = await StartApplication();
  }

  private static async Task<IHost> StartApplication()
  {
    var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
      {
        builder
          .UseStartup<Startup>()
          .UseTestServer()
          .UseEnvironment("Development");
      })
      .Build();
    await host.StartAsync();
    return host;
  }
}