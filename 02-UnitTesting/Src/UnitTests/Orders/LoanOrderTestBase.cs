using System;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;

namespace UnitTests.Orders;

/// <summary>
/// Bazowa klasa testowa, z której dziedziczą klasy testowe w pakiecie.
/// Rozwiązanie polecane przez Olgę i Marcina szczególnie kiedy mamy
/// wiele klas testowych wymagających takiego samego setupu.
/// </summary>
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
        Gender.Male,
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
        Gender.Female,
        "89031013409"));
  }


  // Metody TearDown odpalają siś po każdym teście.
  // W wielu różnych bibliotekach do testów można się też z tego miejsca dostać
  // do metadanych o teście (tu: TestContext.CurrentContext.Test).
  //
  // Oczywiście, można mieć tu jakiś sensowny biznesowy scenariusz zamiast logowania;
  // to co chcemy pokazać, to że narzędzia umożliwiają nam wywołanie danej metody po każdym teście.
  [TearDown]
  protected void AfterEachTest()
  {
    Console.WriteLine($"Finished test: {TestContext.CurrentContext.Test.Name}.");
  }
}