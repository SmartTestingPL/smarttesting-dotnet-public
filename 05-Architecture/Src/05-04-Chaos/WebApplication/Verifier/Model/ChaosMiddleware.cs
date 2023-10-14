using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using WebApplication.Verifier.Infrastructure;

namespace WebApplication.Verifier.Model;

/// <summary>
/// Asp.Net Core'owy middleware
/// (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1)
///
/// Umieściłem w nim polityki Simmy odpowiedzialne za wstrzykiwanie opóźnień przed
/// dostępem do kontrolera WebApi.
/// </summary>
public class ChaosMiddleware
{
  /// <summary>
  /// Następny element łańcucha middleware. Użyty żeby przekazać sterowanie dalej.
  /// </summary>
  private readonly RequestDelegate _next;

  /// <summary>
  /// Polityka Simmy wstrzykująca opóźnienie
  /// </summary>
  private readonly InjectLatencyPolicy _chaosPolicy = MonkeyPolicy.InjectLatency(with =>
    with.Latency((context, token) => TimeSpan.FromMilliseconds(
        Assaults.Config.RandomLatencyFromRange()))
      .InjectionRate(1)
      .EnabledWhen((context, token) => Assaults.Config.EnableLatencyAssault)
  );

  public ChaosMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  /// <summary>
  /// Metoda wykonywana automatycznie przez Asp.Net Core
  /// przed przekazaniem żądania do kontrolera.
  /// </summary>
  /// <param name="context">kontekst żądania</param>
  public Task InvokeAsync(HttpContext context)
  {
    return _chaosPolicy.Execute(async () => await _next.Invoke(context));
  }
}