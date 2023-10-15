using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WebApplication;

namespace WebApplicationTests;

public class ApplicationTests
{
  /// <summary>
  /// Testujemy scenariusz krytyczny weryfikacji oszusta.
  /// Dla oszusta oczekujemy, że status odpowiedzi będzie 401 - UNAUTHORIZED
  /// </summary>
  [Test]
  public async Task ShouldMarkCustomAsFraud()
  {
    using var devApplication = await DevApplication.StartNew();
    var address = devApplication.RetrieveAppBaseUrl();

    var httpResponseMessage = await address.AppendPathSegments("fraud", "fraudCheck")
      .WithHeader("content-type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(Fraud);

    httpResponseMessage.StatusCode.Should()
      .Be((int)HttpStatusCode.Unauthorized);
  }

  /// <summary>
  /// Testujemy scenariusz krytyczny weryfikacji osoby uczciwej.
  /// Dla osoby uczciwej oczekujemy, że status odpowiedzi będzie 200 - OK
  /// </summary>
  [Test]
  public async Task ShouldMarkCustomAsNonFraud()
  {
    using var host = await DevApplication.StartNew();
    var address = host.RetrieveAppBaseUrl();

    using var httpResponseMessage = await address.AppendPathSegments("fraud", "fraudCheck")
      .WithHeader("content-type", "application/json")
      .AllowAnyHttpStatus()
      .PostStringAsync(NonFraud);

    httpResponseMessage.StatusCode.Should().Be((int)HttpStatusCode.OK);
  }

  private static readonly string Fraud = 
    new JObject(
      new JProperty("guid", "cc8aa8ff-40ff-426f-bc71-5bb7ea644108"),
      new JProperty("person", 
        new JObject(
          new JProperty("name", "Fraudeusz"), 
          new JProperty("surname", "Fraudowski"), 
          new JProperty("dateOfBirth", "1980-01-01"), 
          new JProperty("gender", "Male"), 
          new JProperty("nationalIdentificationNumber", "2345678901")))).ToString();

  private static readonly string NonFraud = new JObject(
    new JProperty("guid", "89c878e3-38f7-4831-af6c-c3b4a0669022"),
    new JProperty("person",
      new JObject(
        new JProperty("name", "Stefania"), 
        new JProperty("surname", "Stefanowska"),
        new JProperty("dateOfBirth", "2020-01-01"), 
        new JProperty("gender", "Female"),
        new JProperty("nationalIdentificationNumber", "1234567890")))).ToString();
}

internal class DevApplication : IDisposable
{
  private readonly IHost _host;

  public static async Task<DevApplication> StartNew()
  {
    var host = Host.CreateDefaultBuilder()
      .ConfigureWebHostDefaults(builder =>
        builder
          .UseStartup<Startup>()
          .UseKestrel()
          .UseEnvironment("Development")
          .UseUrls("http://[::1]:0")
      )
      .Build();
    await host.StartAsync();
    var devApplication = new DevApplication(host);
    return devApplication;
  }

  private DevApplication(IHost host)
  {
    _host = host;
  }

  public void Dispose()
  {
    _host.Dispose();
  }

  /// <summary>
  /// Metoda umożliwiająca pobranie adresu, na którym wystartowała nasza aplikacja
  /// </summary>
  public string RetrieveAppBaseUrl()
  {
    var server = _host.Services.GetRequiredService<IServer>();
    var addressFeature = server.Features.Get<IServerAddressesFeature>();
    return addressFeature.Addresses.Single();
  }
}