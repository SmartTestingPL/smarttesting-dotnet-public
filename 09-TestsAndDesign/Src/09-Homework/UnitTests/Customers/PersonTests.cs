using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;

namespace UnitTests.Customers;

[Homework("Zrefaktoruj ten test. Czy na pewno musimy tyle weryfikować?")]
public class PersonTests
{
  [Test]
  public void ShouldWorkWithGetters()
  {
    var person = new Person(
      "name",
      "surname",
      new LocalDate(2001, 11, 1).Just(),
      Gender.Male.Just(),
      "1234567890");

    person.Name.Should().Be("name");
    person.Surname.Should().Be("surname");
    person.Gender.Should().Be(Gender.Male.Just());
    person.NationalIdentificationNumber.Should().Be("1234567890");
    person.GetAge().Should().BeGreaterThanOrEqualTo(9);
  }
}