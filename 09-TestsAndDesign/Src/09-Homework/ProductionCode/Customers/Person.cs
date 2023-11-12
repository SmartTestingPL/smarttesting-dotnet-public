using System;
using Core.Maybe;
using NodaTime;
using ProductionCode.Lib;

namespace ProductionCode.Customers;

public class Person
{
  private Status _status = Status.NotStudent;

  public Person(
    string name,
    string surname,
    Maybe<LocalDate> dateOfBirth,
    Maybe<Gender> gender,
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
  public Maybe<Gender> Gender { get; }
  public string NationalIdentificationNumber { get; }
  public bool IsStudent => Status.Student == _status;

  public void Student()
  {
    _status = Status.Student;
  }

  //Per MS guidelines, getters that can throw exceptions, should not be properties
  public int GetAge()
  {
    var currentDate = CurrentDate;
    return DateOfBirth
      .Select(birthday => (currentDate - birthday).Years)
      .OrElse(() => new InvalidOperationException("Date of birth is required at this point"));
  }

  public virtual LocalDate CurrentDate => Clocks.ZonedUtc.GetCurrentDate();
}