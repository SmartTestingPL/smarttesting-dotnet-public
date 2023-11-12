using System;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;

namespace UnitTests.Orders;

public class LoanOrderTestBase
{
  protected Customer AStudent()
  {
    var customer = new Customer(
      Guid.NewGuid(),
      new Person(
        "Jan",
        "Nowicki",
        new LocalDate(1996, 8, 28).Just(),
        Gender.Male.Just(),
        "96082812079"));
    customer.Student();
    return customer;
  }

  protected Customer ACustomer()
  {
    return new Customer(
      Guid.NewGuid(),
      new Person(
        "Maria",
        "Kowalska",
        new LocalDate(1989, 3, 10).Just(),
        Gender.Female.Just(),
        "89031013409"));
  }

  [TearDown]
  protected void AfterEachTest()
  {
    Console.WriteLine($"Finished test: {TestContext.CurrentContext.Test.Name}.");
  }
}