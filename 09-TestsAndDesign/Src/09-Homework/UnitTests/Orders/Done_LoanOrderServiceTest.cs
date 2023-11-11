using System;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Core.Maybe;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Loans;
using ProductionCode.Orders;

namespace UnitTests.Orders;

/// <summary>
/// Pierwotny test jest mało czytelny. Po pierwsze nazwy metod są niedokładne, a
/// po drugie w możemy lepiej kod sformatować i zastosować assert object, żeby
/// zwiększyć czytelność sekcji then.
/// </summary>
public class Done_LoanOrderServiceTest
{
  private const string StudentPromoDiscountName = "Student Promo";
  private const string DefaultStudentPromoDiscountValue = "100";
  private const string DefaultStudentPromoCommissionValue = "200";
  private const string DefaultStudentPromoLoanAmount = "500";

  [Test]
  public void ShouldThrowExceptionWhenANonStudentWantsToTakeAStudentLoan()
  {
    var loanOrderService = new LoanOrderService(null, null);

    loanOrderService.Invoking(s => s.StudentLoanOrder(NotAStudent()))
      .Should().Throw<Exception>().WithMessage("*Cannot order student loan*");
  }

  [Test]
  public void ShouldGrantAStudentLoanWhenAStudentAppliesForIt()
  {
    var customer = AStudent();
    var loanOrderService = new LoanOrderService(
      Substitute.For<IPostgresAccessor>(),
      StudentPromoReturningMongoDbAccessor());

    var loanOrder = loanOrderService.StudentLoanOrder(customer);

    loanOrder.Should()
      .BeStudentLoan()
      .And.HaveStudentPromotionWithValue(DefaultStudentPromoDiscountValue)
      .And.HaveCommisionEqualTo(DefaultStudentPromoCommissionValue)
      .And.HaveAmountEqualTo(DefaultStudentPromoLoanAmount);
  }

  private IMongoDbAccessor StudentPromoReturningMongoDbAccessor()
  {
    var accessor = Substitute.For<IMongoDbAccessor>();
    accessor.GetPromotionDiscount(StudentPromoDiscountName).Returns(decimal.Parse(DefaultStudentPromoDiscountValue));
    return accessor;
  }

  private Customer AStudent()
  {
    var willBeAStudent = NotAStudent();
    willBeAStudent.Student();
    return willBeAStudent;
  }

  private Customer NotAStudent()
  {
    return new Customer(Guid.NewGuid(),
      new Person("A", "B", Clocks.ZonedUtc.GetCurrentDate().Just(), Gender.Female.Just(), "1234567890"));
  }

  /// <summary>
  /// Używamy FluentAssertions jako wzorca assert object. Klasę umieszczamy tutaj dla lepszej widoczności problemu.
  /// </summary>
  public class LoanOrderAssert : ReferenceTypeAssertions<LoanOrder, LoanOrderAssert>
  {

    public LoanOrderAssert(LoanOrder actual) : base(actual)
    {
      new ObjectAssertions(actual).NotBeNull();
    }

    internal AndConstraint<LoanOrderAssert> BeStudentLoan()
    {
      Execute.Assertion
        .Given(() => Subject.Type)
        .ForCondition(type => type == LoanType.Student)
        .FailWith("Loan type must be of type student, but was {0}", t => t);
      return new AndConstraint<LoanOrderAssert>(this);
    }

    internal AndConstraint<LoanOrderAssert> HaveStudentPromotionWithValue(string value)
    {
      Subject.Promotions.Should().NotBeEmpty();
      value.Should().NotBeNullOrWhiteSpace();
      var promotion = Subject.Promotions[0];
      Execute.Assertion
        .Given(() => promotion.Name)
        .ForCondition(name => StudentPromoDiscountName == name)
        .FailWith("Promotion name should be {0} but was {1}", _ => StudentPromoDiscountName, name => name);
      Execute.Assertion
        .Given(() => decimal.Parse(value))
        .ForCondition(decimalValue => decimalValue == promotion.Discount)
        .FailWith("Promotion value should be {0} but was {1}", decimalValue => decimalValue, _ => promotion.Discount);

      return new AndConstraint<LoanOrderAssert>(this);
    }

    internal AndConstraint<LoanOrderAssert> HaveCommisionEqualTo(string commission)
    {
      commission.Should().NotBeNullOrWhiteSpace();

      Execute.Assertion
        .Given(() => decimal.Parse(commission))
        .ForCondition(decimalValue => decimalValue == Subject.Commission)
        .FailWith("Commission value should be {0} but was {1}",
          decimalValue => decimalValue,
          _ => Subject.Commission);

      return new AndConstraint<LoanOrderAssert>(this);
    }

    internal AndConstraint<LoanOrderAssert> HaveAmountEqualTo(string amount)
    {
      amount.Should().NotBeNullOrWhiteSpace();
      Execute.Assertion
        .Given(() => decimal.Parse(amount))
        .ForCondition(decimalValue => decimalValue == Subject.Amount)
        .FailWith("Loan amount value should be {0} but was {1}",
          decimalValue => decimalValue,
          _ => Subject.Amount);
      return new AndConstraint<LoanOrderAssert>(this);
    }

    protected override string Identifier => this.Subject.GetType().Name;
  }

}

/// <summary>
/// Klasa zawierająca metody fabrykujące - tworzące nasze implementacji wzorca
/// assert object.
/// </summary>
public static class LoanOrderAssertionExtensions
{
  public static Done_LoanOrderServiceTest.LoanOrderAssert Should(this LoanOrder loanOrder)
  {
    return new Done_LoanOrderServiceTest.LoanOrderAssert(loanOrder);
  }
}