using System;
using System.IO;
using System.Threading.Tasks;
using Core.Customers;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace Core.Verifier;

public interface IBikVerificationService
{
  Task<CustomerVerificationResult> Verify(Customer customer);
}

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej
/// za pomocą HTTP.
/// </summary>
public class BikVerificationService : IBikVerificationService
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
  /// wysłanie zapytania po HTTP do BIK. Do wykonania
  /// zapytania po HTTP wykorzystujemy
  /// bibliotekę Flurl.Http.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer)
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
    catch (IOException exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}