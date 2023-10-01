using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej po HTTP.
/// </summary>
public class BikVerificationService
{
  private readonly string _bikServiceBaseUri;
  private readonly ILogger<BikVerificationService> _logger;

  public BikVerificationService(string bikServiceBaseUri, ILogger<BikVerificationService> logger)
  {
    _bikServiceBaseUri = bikServiceBaseUri;
    _logger = logger;
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem poprzez
  /// wysłanie zapytania po HTTP do BIK. Do wykonania zapytania po HTTP wykorzystujemy
  /// bibliotekę Apache HTTP Client.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public virtual async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    try
    {
      var externalStatus = await _bikServiceBaseUri
        .AppendPathSegment(customer.Person.NationalIdentificationNumber)
        .GetStringAsync();

      if (externalStatus == Enum.GetName(
            typeof(VerificationStatus),
            VerificationStatus.VerificationPassed))
      {
        return CustomerVerificationResult.Passed(customer.Guid);
      }
    }
    catch (FlurlHttpException exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}