using System;
using Functional.Maybe.Just;
using NodaTime;
using ProductionCode.Customers;
using ProductionCode.Lib;
using ProductionCode.Orders;

namespace UnitTests.Loans;

internal static class LoanTestUtils
{
  internal static LoanOrder ALoanOrder(
    decimal amount,
    decimal interestRate,
    decimal commission,
    params Promotion[] promotions)
  {
    var customer = new Customer(
      Guid.NewGuid(), 
      new Person(
        "Maria", 
        "Kowalska", 
        new LocalDate(1989, 3, 10).Just(), 
        Gender.Female, 
        "89031013409"));
    var loanOrder = new LoanOrder(
      Clocks.ZonedUtc.GetCurrentDate(), 
      customer, 
      amount, 
      interestRate, 
      commission);
    loanOrder.Promotions.AddRange(promotions);
    return loanOrder;
  }

  internal static LoanOrder ALoanOrder(decimal amount, decimal interestRate, decimal commission)
  {
    var customer = new Customer(Guid
      .NewGuid(), new Person("Maria", "Kowalska",
      new LocalDate(1989, 3, 10).Just(), Gender.Female, "89031013409"));
    return new LoanOrder(Clocks.ZonedUtc.GetCurrentDate(), customer, amount, interestRate, commission);
  }

  internal static LoanOrder ALoanOrder()
  {
    return ALoanOrder(2000, 5, 300);
  }

  internal static LoanOrder ALoanOrder(params Promotion[] promotions)
  {
    return ALoanOrder(2000, 5, 300, promotions);
  }
}