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
  private readonly Gender? _gender;

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
    _gender = null;
    NationalIdentificationNumber = nationalIdentificationNumber;
  }

  public string? Name { get; }
  public string Surname { get; }
  public Maybe<LocalDate> DateOfBirth { get; }

  /// <summary>
  /// W wersji Javowej było tu po prostu zwracanie _gender,
  /// po czym w innym miejscu kodu .toString() wywołany na zwróconym
  /// stąd nullu powodował wyjątek. W C# enumy nie mogą być nullami,
  /// co prowadzi mnie o dylematu. Mógłbym użyć tutaj _gender.Value,
  /// który rzuciłby wyjątkiem w przypadku nulla, ale byłby to
  /// <see cref="InvalidOperationException"/>, a nie jak chce przykład,
  /// <see cref="NullReferenceException"/>. Dlatego też zdecydowałem się
  /// wykorzystać <code>?? throw new NullReferenceException()</code>
  /// jako drobny hack który sprawia, że reszta przykładu wciąż trzyma się kupy.
  /// </summary>
  public Gender GetGender()
  {
    return _gender ?? throw new NullReferenceException();
  }

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

  public override string ToString()
  {
    return "Person{" +
           $"name='{Name}', " +
           $"surname='{Surname}', " +
           $"dateOfBirth={DateOfBirth.Select(d => d.ToString()).OrElse("null")}, " +
           $"gender={GetGender()}, " +
           $"nationalIdentificationNumber='{NationalIdentificationNumber}', " +
           $"status={_status}" +
           "}";
  }
}