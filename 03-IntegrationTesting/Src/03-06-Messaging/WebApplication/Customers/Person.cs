using System;
using Core.Maybe;
using NodaTime;
using WebApplication.Lib;

namespace WebApplication.Customers;

/// <summary>
/// Reprezentuje osobę do zweryfikowania.
/// </summary>
public class Person
{
  private Status _status = Status.NotStudent;

  public Person(
    string name,
    string surname,
    Maybe<LocalDate> dateOfBirth,
    Gender gender,
    string nationalIdentificationNumber)
  {
    Name = name;
    Surname = surname;
    DateOfBirth = dateOfBirth;
    Gender = gender;
    NationalIdentificationNumber = nationalIdentificationNumber;
  }

  public string Name { get; }
  public string Surname { get; }
  public Maybe<LocalDate> DateOfBirth { get; }
  public Gender Gender { get; }
  public string NationalIdentificationNumber { get; }
  public bool IsStudent => Status.Student == _status;

  public void Student()
  {
    _status = Status.Student;
  }

  //Per MS guidelines, getters that can throw exceptions, should not be properties
  public int GetAge()
  {
    var currentDate = Clocks.ZonedUtc.GetCurrentDate();
    return DateOfBirth
      .Select(birthday => (currentDate - birthday).Years)
      .OrElse(() => new InvalidOperationException("Date of birth is required at this point"));
  }

  public override string ToString()
  {
    return
      "Person{" +
      $"name='{Name}', " +
      $"surname='{Surname}', " +
      $"dateOfBirth={DateOfBirth}, " +
      $"gender={Gender}, " +
      $"nationalIdentificationNumber='{NationalIdentificationNumber}', " +
      $"status={_status}}}";
  }
}