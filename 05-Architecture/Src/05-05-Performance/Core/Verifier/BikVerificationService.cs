using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Microsoft.Extensions.Logging;

namespace Core.Verifier;

public interface IBikVerificationService
{
  Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken);
}

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej.
/// </summary>
public class BikVerificationService : IBikVerificationService
{
  private readonly string _bikServiceBaseUri;
  private readonly ILogger<BikVerificationService> _logger;

  public BikVerificationService(
    string bikServiceBaseUri,
    ILogger<BikVerificationService> logger)
  {
    _bikServiceBaseUri = bikServiceBaseUri;
    _logger = logger;
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem. W wersji do tego modułu
  /// zwraca wcześniej przygotowane wartości.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <param name="cancellationToken"></param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
  {
    try
    {
      await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
      return CustomerVerificationResult.Passed(customer.Guid);
    }
    catch (TaskCanceledException exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}