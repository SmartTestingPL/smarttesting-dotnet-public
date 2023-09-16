using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Uproszczony klient do komunikacji z Biurem Informacji Kredytowej
/// z użyciem klienta HTTP.
/// Klasa używana do pokazania jak na poziomie testów jednostkowych unikać
/// komunikacji sieciowej.
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

  ///<summary>
  /// Główna metoda klienta. Weryfikuje czy dana osoba jest oszustem poprzez
  /// wysłanie zapytania po HTTP do BIK. Do wykonania zapytania po HTTP wykorzystujemy
  /// standardową klasę .NETową HttpClient.
  /// 
  /// W Javowej wersji tego przykładu wykorzystany jest klient HTTP który ma błędy.
  /// Te błędy ujawnione są dopiero w tygodniu 3 przez testy integracyjny.
  /// W wersji .NETowej takich problemów z domyślnym klientem HTTP nie ma,
  /// a zatem zostawiłem poprawną implementację. Za to w rozdziale 3 celowo ją "zepsuję".
  ///
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  ///</summary>
  public virtual async Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
  {
    try
    {
      Uri.TryCreate(new Uri(_bikServiceBaseUri), customer.Person.NationalIdentificationNumber, out var requestUri);
      var response = await _client.GetAsync(requestUri, cancellationToken);
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