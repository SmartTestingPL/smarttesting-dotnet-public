using System;
using System.Net;
using System.Threading.Tasks;
using E2ETests.Customers;
using E2ETests.Orders;
using FluentAssertions;
using FluentAssertions.Extensions;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;

namespace E2ETests;

/// <summary>
/// Przykład klasy basowej dla testów E2E po HTTP. Wszystkie
/// szczegóły związane z implementacją warstwy integracyjnej
/// zostały wyniesione do metod pomocniczych przez co testy
/// są o wiele bardziej czytelne.
/// </summary>
public class LoanOrdersHttpClientTestBase : LoanOrdersTestBase
{

  protected static FlurlClient HttpClient = default!;
  private static JsonSerializerSettings _jsonSettings = default!;

  /// <summary>
  /// Tworzenie klienta HTTP z odpowiednimi
  /// ustawieniami serializacji JSONa
  /// </summary>
  [OneTimeSetUp]
  public static void SetUpAll()
  {
    _jsonSettings = new JsonSerializerSettings
    {
      MissingMemberHandling = MissingMemberHandling.Error
    };
    HttpClient = new FlurlClient
    {
      BaseUrl = LoanOrdersUri,
      Settings =
      {
        JsonSerializer = new NewtonsoftJsonSerializer(_jsonSettings)
      }
    };
  }

  /// <summary>
  /// Czyszczenie zasobów
  /// </summary>
  [OneTimeTearDown]
  public static void TearDownAll()
  {
    HttpClient.Dispose();
  }

  /// <summary>
  /// Szczegóły techniczne wysyłania POSTa wyniesione do metody pomocniczej
  /// </summary>
  protected static async Task<IFlurlResponse> IssuePost(Customer customer)
  {
    return await HttpClient.Request()
      .WithHeader("Content-type", "application/json")
      .AllowAnyHttpStatus()
      .PostJsonAsync(new LoanOrder(customer));
  }


  protected static async Task<LoanOrder> CreateAndRetrieveLoanOrderAsync(Task<IFlurlResponse> httpPost)
  {
    (await httpPost).StatusCode.Should().Be((int)HttpStatusCode.OK);

    var loanOrderId = (await httpPost.ReceiveJson<LoanOrderId>()).Data;

    var loanOrderRequest = HttpClient.Request(loanOrderId)
      .WithHeader("Accept", "application/json");
    
    // Zastosowanie odpytywania Polly w celu uniknięcia
    // false negatives
    await Policy.Handle<Exception>()
      .WaitAndRetryAsync(6000, retryAttempt => 10.Milliseconds())
      .ExecuteAsync(async () =>
      {
        var httpResponse = await loanOrderRequest.AllowAnyHttpStatus().GetAsync();
        httpResponse.StatusCode.Should().Be((int)HttpStatusCode.OK);
      });

    var receivedLoanOrder = await loanOrderRequest.GetJsonAsync<LoanOrder>();
    return receivedLoanOrder;
  }
}