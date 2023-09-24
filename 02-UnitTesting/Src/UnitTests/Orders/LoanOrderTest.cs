using System;
using FluentAssertions;
using NUnit.Framework;
using ProductionCode.Lib;
using ProductionCode.Orders;

namespace UnitTests.Orders;

/// <summary>
/// Przykład zastosowania klasy bazowej w celu zwiększenia czytelności i umożliwienia reużycia kodu.
/// Przykład testowania stanu.
/// </summary>
public class LoanOrderTest : LoanOrderTestBase
{
  // Testowanie stanu
  [Test]
  public void ShouldAddManagerPromo()
  {
    var loanOrder = new LoanOrder(Clocks.ZonedUtc.GetCurrentDate(), ACustomer());
    var managerGuid = Guid.NewGuid();

    loanOrder.AddManagerDiscount(managerGuid);

    loanOrder.Promotions.Should().HaveCount(1);
    loanOrder.Promotions[0].Name.Should().Contain(managerGuid.ToString());
    loanOrder.Promotions[0].Discount.Should().Be(50);
  }
}