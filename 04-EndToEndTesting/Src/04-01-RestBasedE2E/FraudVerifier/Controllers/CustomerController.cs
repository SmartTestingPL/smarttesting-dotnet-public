using FraudVerifier.Customers;
using Microsoft.AspNetCore.Mvc;

namespace FraudVerifier.Controllers;

/// <summary>
/// Kontroler umożliwiający wykonywanie operacji na klientach.
/// </summary>
[ApiController]
[Route("customers")]
public class CustomerController : ControllerBase
{
  private readonly CustomerVerifier _verifier;

  public CustomerController(CustomerVerifier verifier)
  {
    _verifier = verifier;
  }

  [HttpPost]
  public CustomerVerificationResult Verify(Customer customer)
  {
    return _verifier.Verify(customer);
  }
}