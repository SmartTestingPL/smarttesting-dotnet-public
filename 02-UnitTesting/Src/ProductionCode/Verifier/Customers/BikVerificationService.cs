using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Uproszczony klient do komunikacji z Biurem Informacji Kredytowej z użyciem klienta HTTP.
///
/// Klasa używana do pokazania jak na poziomie testów jednostkowych unikać komunikacji sieciowej.
/// </summary>
public class BikVerificationService
{
  private readonly string _bikServiceBaseUri;
  private readonly HttpClient _client;
  private readonly ILogger<BikVerificationService> _logger;

  public BikVerificationService(string bikServiceBaseUri, ILogger<BikVerificationService> logger)
  {
    _bikServiceBaseUri = bikServiceBaseUri;
    _logger = logger;
    _client = new HttpClient();
  }

  public virtual async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    try
    {
      Uri.TryCreate(new Uri(_bikServiceBaseUri), customer.Person.NationalIdentificationNumber, out var requestUri);
      var response = await _client.GetAsync(requestUri);
      var externalStatus = await response.Content.ReadAsStringAsync();

      if (externalStatus == Enum.GetName(
            typeof(VerificationStatus),
            VerificationStatus.VerificationPassed))
      {
        return CustomerVerificationResult.Passed(customer.Guid);
      }
    }
    catch (IOException exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}