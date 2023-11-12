using System;
using FluentAssertions;
using Core.Maybe;
using NodaTime;
using NUnit.Framework;
using ProductionCode.Customers;

namespace UnitTests.Customers;

/// <summary>
/// Pierwotny test testował gettery i settery, które jedynie zwracały ustawioną wartość.
/// To, co na pewno powinniśmy przetestować to sposób liczenia wieku - tam nie jest zwracana 
/// ustawiona wartość wieku tylko jest on wyliczony.
/// </summary>
public class Done_PersonTests
{

  /// <summary>
  /// Przykład udanego wyliczenia wieku.
  /// </summary>
  [Test]
  public void ShouldCalculateAgeOfPerson()
  {
    Person person = new PersonStoppedInTime("name",
      "surname",
      new LocalDate(2001, 11, 1).Just(),
      Gender.Male,
      "1234567890",
      currentDate: new LocalDate(2011, 11, 1));

    person.GetAge().Should().Be(10);
  }

  /// <summary>
  /// Przykład wyliczenia wieku, który zakończy się rzuceniem wyjątku.
  /// </summary>
  [Test]
  public void ShouldFailToCalculateTheAgeOfPersonWhenDateIsNull()
  {
    var person = new Person("name", "surname", Maybe<LocalDate>.Nothing, Gender.Male.Just(), "1234567890");

    person.Invoking(p => p.GetAge()).Should().ThrowExactly<InvalidOperationException>();
  }

  private class PersonStoppedInTime : Person
  {
    public PersonStoppedInTime(
      string name,
      string surname,
      Maybe<LocalDate> dateOfBirth,
      Gender gender,
      string nationalIdentificationNumber, LocalDate currentDate)
      : base(name, surname, dateOfBirth, gender.Just(), nationalIdentificationNumber)
    {
      CurrentDate = currentDate;
    }

    public override LocalDate CurrentDate { get; }
  }
}