using System;
using Core.Maybe;
using NodaTime;
using ProductionCode.Lib;

namespace ProductionCode.Client;

/// <summary>
/// Reprezentuje osobę do zweryfikowania.
/// </summary>
public class Person
{
  private PersonStatus _status = PersonStatus.NotStudent;

  public Person(
    string name,
    string surname,
    Maybe<LocalDate> dateOfBirth,
    Gender gender,
    string nationalIdentificationNumber,
    Guid guid)
  {
    Name = name;
    Surname = surname;
    DateOfBirth = dateOfBirth;
    Gender = gender;
    NationalIdentificationNumber = nationalIdentificationNumber;
    Guid = guid;
  }

  public string Name { get; }
  public string Surname { get; }
  public Maybe<LocalDate> DateOfBirth { get; }
  public Gender Gender { get; }
  public string NationalIdentificationNumber { get; }
  public bool IsStudent => PersonStatus.Student == _status;
  public Guid Guid { get; }

  public void Student()
  {
    _status = PersonStatus.Student;
  }

  public int GetAge()
  {
    var currentDate = Clocks.ZonedUtc.GetCurrentDate();
    return DateOfBirth
      .Select(birthday => (currentDate - birthday).Years)
      .OrElse(() => new InvalidOperationException("Date of birth is required at this point"));
  }
}