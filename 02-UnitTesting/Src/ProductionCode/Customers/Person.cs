using System;
using Core.Maybe;
using NodaTime;
using ProductionCode.Lib;

namespace ProductionCode.Customers;

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

  /// <summary>
  /// Metoda zaimplementowana jako getter, nie własność, gdyż
  /// wytyczne Microsoftu są takie, żeby własności nie mogły rzucać wyjątków.
  /// </summary>
  /// <exception cref="InvalidOperationException">jeśli nie ma daty urodzin</exception>
  public int GetAge()
  {
    var currentDate = Clocks.ZonedUtc.GetCurrentDate();
    return DateOfBirth
      .Select(birthday => (currentDate - birthday).Years)
      .OrElse(() => new InvalidOperationException("Date of birth is required at this point"));
  }
}