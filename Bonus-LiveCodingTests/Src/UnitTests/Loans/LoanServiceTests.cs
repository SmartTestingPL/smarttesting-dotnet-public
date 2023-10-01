using System;
using System.Collections.Generic;
using FluentAssertions;
using NodaTime;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Db;
using ProductionCode.Events;
using ProductionCode.Loans;
using ProductionCode.Loans.Validation;
using ProductionCode.Orders;
using TddXt.XNSubstitute;
using static UnitTests.Loans.LoanTestUtils;

namespace UnitTests.Loans;

public class LoanServiceTests
{
  private IEventEmitter _eventEmitter = default!;
  private IMongoDbAccessor _mongoDbAccessor = default!;
  private LoanService _loanService = default!;

  [SetUp]
  public void SetUp()
  {
    _eventEmitter = Substitute.For<IEventEmitter>();
    _mongoDbAccessor = Substitute.For<IMongoDbAccessor>();
    _loanService = new LoanService(_eventEmitter, _mongoDbAccessor, new TestPostgresAccessor());
    _mongoDbAccessor.GetMinCommission().Returns(200);
  }

  [Test]
  public void ShouldCreateLoan()
  {
    var loanOrder = ALoanOrder();

    var loan = _loanService.CreateLoan(loanOrder, 3);

    loan.Guid.Should().NotBe(Guid.Empty);
  }

  [Test]
  public void ShouldEmitEventWhenLoanCreated()
  {
    var loanOrder = ALoanOrder();
    LoanCreatedEvent? emittedEvent = null;
    _eventEmitter.Emit(Arg.Do<LoanCreatedEvent>(@event => emittedEvent = @event));

    _loanService.CreateLoan(loanOrder, 3);

    emittedEvent.Should().NotBeNull();
    emittedEvent!.LoanGuid.Should().NotBe(Guid.Empty);
    // eventEmitter.ReceivedNothing();
    XReceived.Only(() => { _eventEmitter.Emit(emittedEvent); });
  }

  [Test, TestCaseSource(nameof(_commissionValues))]
  public void ShouldThrowExceptionWhenIncorrectCommission(decimal commission)
  {
    _loanService.Invoking(s => s.CreateLoan(ALoanOrder(
        2000, 5, commission), 5))
      .Should().ThrowExactly<CommissionValidationException>();
  }

  [Test]
  public void ShouldNotThrowExceptionIfEmptyPromotions()
  {
    _loanService.Invoking(s => s.CreateLoan(ALoanOrder(Array.Empty<Promotion>()), 6))
      .Should().NotThrow();
  }

  [Test]
  public void ShouldRemoveIncorrectPromotions()
  {
    var loanOrder = ALoanOrder(new Promotion("promotion not in DB", 55));

    _loanService.UpdatePromotions(loanOrder);

    loanOrder.Promotions.Should().BeEmpty();
  }

  private static object[] _commissionValues = { decimal.Zero, new decimal(-1), new decimal(199) };
}

class TestPostgresAccessor : IPostgresAccessor
{
  public void UpdatePromotionStatistics(string promotionName)
  {

  }

  public void UpdatePromotionDiscount(string promotionName, decimal newDiscount)
  {
  }

  public IList<Promotion> GetValidPromotionsForDate(LocalDate localDate)
  {
    return new List<Promotion>
    {
      new("test 10", 10),
      new("test 20", 20)
    };
  }
}