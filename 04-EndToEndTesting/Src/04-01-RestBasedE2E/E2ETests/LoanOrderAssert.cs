using E2ETests.Orders;
using FluentAssertions;

namespace E2ETests;

/// <summary>
/// Przykład zastosowania wzorca AssertObject.
/// </summary>
public class LoanOrderAssert
{
  private readonly LoanOrder _loanOrder;

  public LoanOrderAssert(LoanOrder loanOrder)
  {
    _loanOrder = loanOrder;
  }

  public void CustomerVerificationPassed()
  {
    _loanOrder.Status.Should().Be(LoanOrder.OrderStatus.Verified);
  }

  public void CustomerVerificationFailed()
  {
    _loanOrder.Status.Should().Be(LoanOrder.OrderStatus.Rejected);
  }
}