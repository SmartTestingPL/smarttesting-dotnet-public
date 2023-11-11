using System;
using FluentAssertions;
using NUnit.Framework;
using ProductionCode.Lib;
using ProductionCode.Orders;

namespace UnitTests.Orders;

public class LoanOrderTest : LoanOrderTestBase
{
  [Test]
  public void ShouldAddManagerPromo()
  {
    var loanOrder = new LoanOrder(Clocks.ZonedUtc.GetCurrentDate(), ACustomer());
    var managerUuid = Guid.NewGuid();

    loanOrder.AddManagerDiscount(managerUuid);

    loanOrder.Promotions.Should().HaveCount(1);
    loanOrder.Promotions[0].Name.Should().Contain(managerUuid.ToString());
    loanOrder.Promotions[0].Discount.Should().Be(50);
  }
}