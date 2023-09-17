using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtmaFileSystem;
using FraudDetection;
using FraudDetection.Customers;
using FraudDetection.Verifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PactNet.Verifier;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 05-03
/// <summary>
/// Odpowiednik Javowej klasy FraudControllerBase
/// </summary>
public class FraudContractTests
{
  private IHost _host = default!;

  [SetUp]
  // Dotyczy lekcji 05-03 i 08-03
  public async Task SetUp()
  {
    // Dotyczy lekcji 05-03
    _host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          .UseKestrel()
          .UseEnvironment("LocalDev")
          .UseUrls("http://[::1]:0")
          .ConfigureTestServices(collection =>
          {
            collection.Replace(
              new ServiceDescriptor(
                typeof(ICustomerVerifier),
                typeof(FakeCustomerVerifier),
                ServiceLifetime.Singleton));
          })
      )
      .Build();
    await _host.StartAsync();
  }

  [TearDown]
  public async Task TearDown()
  {
    await _host.StopAsync();
    _host.Dispose();
  }

  [Test]
  public void ShouldHonourPactWithConsumer()
  {
    var pactVerifier = new PactVerifier(
      new PactVerifierConfig {
        Outputters = new List<PactNet.Infrastructure.Outputters.IOutput> {
          new PactNet.Infrastructure.Outputters.ConsoleOutput()
        }
      });
    pactVerifier
      .ServiceProvider("FraudDetection", new Uri(RetrieveAppBaseUrl()))
      .WithFileSource(
        AbsoluteFilePath.OfThisFile()
          .ParentDirectory(3).Value()
          .AddDirectoryName("Contracts")
          .AddDirectoryName("Http")
          .AddFileName("FraudDetectionConsumer-FraudDetection.json").Info())
      .Verify();
  }

  /// <summary>
  /// Metoda umożliwiająca pobranie adresu, na którym wystartowała nasza aplikacja
  /// </summary>
  public string RetrieveAppBaseUrl()
  {
    var server = _host.Services.GetRequiredService<IServer>();
    var addressFeature = server.Features.Get<IServerAddressesFeature>();
    return addressFeature!.Addresses.Single();
  }
}

public class FakeCustomerVerifier : CustomerVerifier
{
  public FakeCustomerVerifier() 
    : base(
      null!, 
      new SimpleObjectProvider<IReadOnlySet<IVerification>>(null!),
      null!, 
      null!,
      NullLogger<CustomerVerifier>.Instance)
  {
  }

  public override async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    if (customer.Person.Name == "Stefania")
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }
    return CustomerVerificationResult.Failed(customer.Guid);
  }
}
