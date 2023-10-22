using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Model;

/// <summary>
/// Klient do komunikacji z Biurem Informacji Kredytowej.
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
    _client = new();
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <param name="cancellationToken"></param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(
    Customer customer, CancellationToken cancellationToken)
  {
    try
    {
      await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
      return Pass(customer);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Http request execution failed.");
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }

  protected virtual CustomerVerificationResult Pass(Customer customer)
  {
    return CustomerVerificationResult.Passed(customer.Guid);
  }

  /// <summary>
  /// Bardzo skomplikowana metoda wykorzystana na slajdach
  /// w celu ukazania wysokiej złożoności cyklomatycznej.
  /// </summary>
  public int ComplexMethod(int a, int b, int c)
  {
    var d = a + 2;
    var e = a > 0 ? d + 5 : c;
    var f = d > 0 ? e + 5 : a;
    var result = 0;
    if (a > b || f > 1 && d + 1 > 3 || f < 4)
    {
      return 8;
    }

    if (a > c && e > f || a > 1 && e + 1 > 3 || d < 4)
    {
      return 1;
    }
    else
    {
      if (a + 1 > c - 1 || a > b + 3 || f > 19)
      {
        return 1233;
      }

      if (e < a && d > c)
      {
        if (a + 4 > b - 2)
        {
          if (c - 5 < a + 11)
          {
            return 81;
          }
          else if (a > c)
          {
            return 102;
          }
        }

        if (a > c + 21 && e > f - 12)
        {
          return 13;
        }
        else
        {
          if (a + 10 > c - 1)
          {
            return 123;
          }
          else if (e + 1 < a && d + 14 > c)
          {
            return 111;
          }

          if (f > 10)
          {
            return 1;
          }
        }
      }
    }

    return result;
  }

}