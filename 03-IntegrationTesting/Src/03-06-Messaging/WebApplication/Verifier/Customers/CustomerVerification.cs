using Newtonsoft.Json;
using WebApplication.Customers;

namespace WebApplication.Verifier.Customers;

/// <summary>
/// Obiekt, który wysyłamy poprzez brokera.
/// Reprezentuje osobę i rezultat weryfikacji.
/// </summary>
public class CustomerVerification
{
  public CustomerVerification(
    [JsonProperty("person")] Person person,
    [JsonProperty("result")] CustomerVerificationResult result)
  {
    Person = person;
    Result = result;
  }

  public Person Person { set; get; }
  public CustomerVerificationResult Result { set; get; }

  public override string ToString()
  {
    return $"CustomerVerification{{person={Person}, result={Result}}}";
  }
}