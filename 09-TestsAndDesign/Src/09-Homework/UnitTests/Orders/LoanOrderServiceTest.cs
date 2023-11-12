using System;
using FluentAssertions;
using Core.Maybe;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Loans;
using ProductionCode.Orders;

namespace UnitTests.Orders;

[Homework("Na pewno mo¿emy popracowaæ nad czytelnoœcia tych testów")]
public class LoanOrderServiceTest
{
  [Test]
  public void TestNotStudent() {
    var loanOrderService = new LoanOrderService(null, null);

    loanOrderService.Invoking(s => s.StudentLoanOrder(
        new Customer(Guid.NewGuid(),
          new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Female.Just(), "1234567890"))))
      .Should().Throw<Exception>().WithMessage("*Cannot order student loan*");
  }

  [Test]
  public void TestStudent() {
    var customer = new Customer(Guid.NewGuid(),
      new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Female.Just(), "1234567890"));
      
    customer.Student();
    var accessor = Substitute.For<IMongoDbAccessor>();
    accessor.GetPromotionDiscount("Student Promo").Returns(decimal.Parse("100"));
    var loanOrderService = new LoanOrderService(Substitute.For<IPostgresAccessor>(), accessor);
    var studentLoanOrder = loanOrderService.StudentLoanOrder(customer);
    studentLoanOrder.Type.Should().Be(LoanType.Student);
    studentLoanOrder.Promotions[0].Should().Be(new Promotion("Student Promo", decimal.Parse("100")));
    studentLoanOrder.Commission.Should().Be(decimal.Parse("200"));
    studentLoanOrder.Amount.Should().Be(decimal.Parse("500"));
  }
}