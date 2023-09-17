using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FraudDetection.Customers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FraudDetection.Verifier;

/// <summary>
/// Interfejs klienta do komunikacji z Biurem Informacji Kredytowej.
/// </summary>
public interface IBikVerificationService
{
  Task<CustomerVerificationResult> Verify(Customer customer);
}

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej po HTTP.
/// </summary>
public class BikVerificationService : IBikVerificationService
{
  private readonly string _bikServiceBaseUri;
  private readonly ILogger<BikVerificationService> _logger;

  public BikVerificationService(
    IOptions<BikServiceOptions> options, 
    ILogger<BikVerificationService> logger)
  {
    _bikServiceBaseUri = options.Value.BaseUrl;
    _logger = logger;
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem poprzez
  /// wysłanie zapytania po HTTP do BIK. Do wykonania zapytania
  /// po HTTP wykorzystujemy bibliotekę Flurl.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    try
    {
      var verificationResultAsString = await _bikServiceBaseUri
        .AppendPathSegment(customer.Person.NationalIdentificationNumber)
        .GetStringAsync();
      var verificationResultValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(verificationResultAsString);

      if (verificationResultValues["status"] == Enum.GetName(VerificationStatus.VerificationPassed))
      {
        return CustomerVerificationResult.Passed(customer.Guid);
      }
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}