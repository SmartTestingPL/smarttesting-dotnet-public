﻿using System;
using Core.Maybe;
using Core.Maybe.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

  [JsonConverter(typeof(MaybeConverter<LocalDate>))]
  public Maybe<LocalDate> DateOfBirth { get; }

  [JsonConverter(typeof(StringEnumConverter))]
  public Gender Gender { get; }
  public string NationalIdentificationNumber { get; }

  [JsonIgnore]
  public bool IsStudent => Status.Student == _status;

  public void Student()
  {
    _status = Status.Student;
  }

  /// <summary>
  /// Zgodnie z wytycznymi Microsoftu, właśności nie powinny rzucać wyjątków,
  /// a zatem wiek pobieramy przez metodę.
  /// </summary>
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