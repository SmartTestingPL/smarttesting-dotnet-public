using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Orders;

namespace UnitTests.Orders;

public class LoanOrderServiceTest
{
  private LoanOrderService _loanOrderService = default!;
    
  [SetUp]
  public void SetUp()
  {
    _loanOrderService = new LoanOrderService();
  }

  // Wywołanie metody wytwórczej w teście.
  [Test]
  public void ShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.OrderDate.Should().Be(Clocks.ZonedUtc.GetCurrentDate());
    loanOrder.Promotions.Should().ContainSingle(promotion => promotion.Name == "Student Promo");
    loanOrder.Promotions[0].Discount.Should().Be(10);
  }

  // Przykład zastosowania AssertObject Pattern
  [Test]
  public void AssertObjectShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    var orderShould = new LoanOrderAssert(loanOrder);
    orderShould.BeRegisteredToday();
    orderShould.HavePromotion("Student Promo");
    orderShould.HaveOnlyOnePromotion();
    orderShould.HaveOnFirstPromotionDiscountValueOf(10);
  }

  // Przykład AssertObject Pattern z łańcuchowaniem asercji.
  // Można by też zrobić z tego rozszerzenie do FluentAssertions,
  // ale nie ma takiego przymusu.
  [Test]
  public void ChainedAssertObjectShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.Should().BeRegisteredToday()
      .HavePromotion("Student Promo")
      .HaveOnlyOnePromotion()
      .HaveOnFirstPromotionDiscountValueOf(10);
  }

  // Przykład AssertObject Pattern z zastosowaniem metody
  // rozszerzającej Should() opakowującej łańcuch asercji
  [Test]
  public void ChainedAssertObjectShouldCreateStudentLoanOrderSimpleAssertion()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.Should().BeCorrect();
  }

  /// <summary>
  /// Metoda zawierająca setup klientów typu Student na potrzeby testów
  /// </summary>
  private static Customer AStudent()
  {
    var customer = new Customer(
      Guid.NewGuid(),
      new Person(
        "John",
        "Smith",
        new LocalDate(1996, 8, 28).Just(),
        Gender.Male,
        "96082812079"));

    customer.Student();

    return customer;
  }
}