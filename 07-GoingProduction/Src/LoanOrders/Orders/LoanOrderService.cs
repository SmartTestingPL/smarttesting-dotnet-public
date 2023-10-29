using System.Threading.Tasks;
using Core.NullableReferenceTypesExtensions;
using LoanOrders.Frauds;
using LoanOrders.Repository;
using Microsoft.Extensions.Logging;

namespace LoanOrders.Orders;

/// <summary>
/// Serwis do realizacji operacji na wnioskach kredytowych.
/// </summary>
public class LoanOrderService
{
  private readonly LoanOrderRepository _loanOrderRepository;
  private readonly FraudWebClient _fraudWebClient;
  private readonly ILogger<LoanOrderService> _logger;

  public LoanOrderService(
    LoanOrderRepository loanOrderRepository,
    FraudWebClient fraudWebClient,
    ILogger<LoanOrderService> logger)
  {
    _loanOrderRepository = loanOrderRepository;
    _fraudWebClient = fraudWebClient;
    _logger = logger;
  }

  public async Task<string> VerifyLoanOrder(LoanOrder loanOrder)
  {
    var customer = loanOrder.Customer;
    var customerVerificationResult = await _fraudWebClient.VerifyCustomer(customer);

    if (VerificationStatus.VerificationFailed == customerVerificationResult.Status)
    {
      _logger.LogWarning("Customer {} has not passed verification", customer.Guid);
      var status = UpdateStatus(loanOrder, LoanOrder.CustomerOrderStatus.Rejected);
      var savedLoanOrder = await _loanOrderRepository.Save(status);
      return savedLoanOrder.Id.OrThrow();
    }
    else
    {
      var status = UpdateStatus(loanOrder, LoanOrder.CustomerOrderStatus.Verified);
      var savedLoanOrder = await _loanOrderRepository.Save(status);
      return savedLoanOrder.Id.OrThrow();
    }
  }

  public async Task<LoanOrder> FindOrder(string orderId)
  {
    return await _loanOrderRepository.FindById(orderId);
  }

  private LoanOrder UpdateStatus(LoanOrder loanOrder, LoanOrder.CustomerOrderStatus status)
  {
    loanOrder.Status = status;
    return loanOrder;
  }
}