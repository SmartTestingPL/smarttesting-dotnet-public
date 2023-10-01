using System;
using Core.Maybe;
using Core.Maybe.Json;
using Newtonsoft.Json;
using NodaTime;
using WebApplication.Lib;

namespace WebApplication.Client;

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
  [JsonConverter(typeof(MaybeConverter<LocalDate>))]
  public Maybe<LocalDate> DateOfBirth { get; }
  public Gender Gender { get; }
  public string NationalIdentificationNumber { get; }
  public Guid Guid { get; }

  [JsonIgnore]
  public bool IsStudent => PersonStatus.Student == _status;

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

  public override string ToString()
  {
    return $"Person{{age={GetAge()}}}";
  }
}