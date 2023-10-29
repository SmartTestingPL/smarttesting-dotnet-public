using System.Threading.Tasks;
using LoanOrders.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoanOrders.Controllers;

/// <summary>
/// Kontroler pozwalający na realizację operacji na wnioskach kredytowych.
/// </summary>
[ApiController]
[Route("orders")]
public class LoanOrderController : ControllerBase
{
  private readonly LoanOrderService _loanOrderService;
  private readonly ILogger<LoanOrderController> _logger;

  public LoanOrderController(LoanOrderService loanOrderService, ILogger<LoanOrderController> logger)
  {
    _loanOrderService = loanOrderService;
    _logger = logger;
  }

  [HttpPost]
  public async Task<IActionResult> CreateOrder(LoanOrder loanOrder)
  {
    var id = await _loanOrderService.VerifyLoanOrder(loanOrder);
    return Ok(new { data = id });
  }

  [HttpGet("{orderId}")]
  public async Task<LoanOrder> FindOrder(string orderId)
  {
    var order = await _loanOrderService.FindOrder(orderId);
    return order;
  }

  [HttpGet]
  public Task<string> Test()
  {
    return Task.FromResult("test");
  }
}