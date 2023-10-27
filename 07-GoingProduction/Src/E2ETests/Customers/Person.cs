using System;
using E2ETests.Lib;
using Core.Maybe;
using Core.Maybe.Json;
using Newtonsoft.Json;
using NodaTime;

namespace E2ETests.Customers;

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

  /// <summary>
  /// Typ Maybe wymaga specjalnego konwertera.
  /// </summary>
  [JsonConverter(typeof(MaybeConverter<LocalDate>))]
  public Maybe<LocalDate> DateOfBirth { get; }
  public Gender Gender { get; }
  public string NationalIdentificationNumber { get; }
  public bool IsStudent => Status.Student == _status;

  public void Student()
  {
    _status = Status.Student;
  }

  public int GetAge()
  {
    var currentDate = Clocks.ZonedUtc.GetCurrentDate();
    return DateOfBirth
      .Select(birthday => (currentDate - birthday).Years)
      .OrElse(() => new InvalidOperationException("Date of birth is required at this point"));
  }
}