using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using LoanOrders.Customers;

namespace LoanOrders.Frauds;

/// <summary>
/// Klient do komunikacji z serwisem Fraud.
/// </summary>
public class FraudWebClient
{
  private readonly IFlurlClient _client;

  public FraudWebClient(HttpClient client)
  {
    _client = new FlurlClient(client)
    {
      BaseUrl = "http://fraud-verifier"
    };
  }

  public Task<CustomerVerificationResult> VerifyCustomer(Customer customer)
  {
    return _client.Request("customers")
      .PostJsonAsync(customer)
      .ReceiveJson<CustomerVerificationResult>();
  }
}