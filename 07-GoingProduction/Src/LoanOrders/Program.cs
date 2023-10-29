using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LoanOrders;

public class Program
{
  public static void Main(string[] args)
  {
    var host = CreateHostBuilder(args)
      .UseSerilog((context, configuration) => configuration
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                         "{Properties:j}{NewLine}{Exception}")
        .Enrich.FromLogContext())
      .Build();
    host.Run();
  }

  private static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}