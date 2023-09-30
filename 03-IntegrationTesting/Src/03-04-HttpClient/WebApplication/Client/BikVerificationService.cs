using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace WebApplication.Client;

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej. Posiada dwa konstruktory:
/// jeden, przyjmujący klienta HTTP, drugi tworzący go na podstawie wartości domyślnych.
/// 
/// W tej implementacji chcemy pokazać jakie problemy można zaobserwować w momencie,
/// w którym nie weryfikujemy jakie wartości domyślne są używane przez nasze narzędzia.
/// </summary>
public class BikVerificationService
{
  private readonly string _bikServiceUri;
  private readonly IFlurlClient _client;
  private readonly ILogger<BikVerificationService> _logger;

  public BikVerificationService(
    string bikServiceUri,
    IFlurlClient client,
    ILogger<BikVerificationService> logger)
  {
    _bikServiceUri = bikServiceUri;
    _client = client;
    _logger = logger;
  }

  // Konstruktor na potrzeby testu. W wersji Java ustawiał klienta HTTP z wartościami domyślnymi
  // i pokazywał, że używanie tych wartości domyślnych np. w Apache Http Client, może się źle skończyć.
  // w wersji .NET specjalnie "psuję" klienta http, dając mu bardzo długi czas wygaśnięcia żądania.
  public BikVerificationService(string bikServiceUri, ILogger<BikVerificationService> logger)
  {
    _bikServiceUri = bikServiceUri;
    _logger = logger;
    // tworzymy klienta http z bardzo długim czasem wygaśnięcia żądania
    // - jest zepsuty celowo, żeby naśladować zachowanie z przykładu Javowego.
    _client = new FlurlClient
    {
      Settings =
      {
        Timeout = Timeout.InfiniteTimeSpan
      }
    };
  }


  /// <summary>
  /// Główna metoda biznesowa. Weryfikuje czy dana osoba jest oszustem poprzez
  /// wysłanie zapytania po HTTP do BIK. Do wykonania zapytania po HTTP wykorzystujemy
  /// HttpClienta.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    try
    {
      // W przypadku domyślnego clienta, to zapytanie może trwać nieskończoność
      var externalStatus = await _client
        .Request(_bikServiceUri)
        .AppendPathSegment(customer.Person.NationalIdentificationNumber)
        .GetStringAsync();

      if (Enum.GetName(
            typeof(VerificationStatus),
            VerificationStatus.VerificationPassed) == externalStatus)
      {
        return CustomerVerificationResult.Passed(customer.Guid);
      }
    }
    catch (FlurlHttpException exception)
    {
      // wyłapujemy wyjątek związany z połączeniem i chcemy go przeprocesować
      ProcessException(exception);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }



  /// <summary>
  /// Domyślna implementacja loguje wyjątek do konsoli. Wyjątek nie jest ponownie rzucany.
  /// </summary>
  /// <param name="exception">wyjątek do obsłużenia</param>
  protected virtual void ProcessException(Exception exception)
  {
    _logger.LogError(exception, "Http request execution failed.");
  }

}