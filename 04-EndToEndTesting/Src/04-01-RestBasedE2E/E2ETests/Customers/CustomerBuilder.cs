using System;
using E2ETests.Lib;
using Core.Maybe;
using NodaTime;

namespace E2ETests.Customers;

/// <summary>
/// Przykład budowniczego do przygotowania danych testowych.
/// </summary>
public class CustomerBuilder
{
  private Guid _guid = Guid.NewGuid();
  private string _name = "Anna";
  private string _surname = "Kowalska";
  private LocalDate _dateOfBirth = new LocalDate(1978, 9, 12);
  private Gender _gender = Gender.Female;
  private string _nationalIdentificationNumber = "78091211463";
  private Status _status = Status.NotStudent;

  public static CustomerBuilder Create()
  {
    return new CustomerBuilder();
  }

  /// <summary>
  /// Klient z generowanymi wartościami.
  /// W wersji Javowej było tu użyte narzędzie JFairy.
  /// Są podobne narzędzia do .NETa, ale nie znalazłem takiego,
  /// które miałoby generowanie PESELu, więc sportowałem ten algorytm.
  /// Znajduje się on w klasie
  /// <see cref="PlNationalIdentificationNumberProvider"/>.
  /// </summary>
  public CustomerBuilder AdultMale()
  {
    var dateOfBirth = DateOfBirthWithMinAgeOf(21);
    return WithName(Generate.AnyValidMaleName())
      .WithSurname(Generate.AnyValidSurname())
      .WithDateOfBirth(dateOfBirth)
      .WithGender(Gender.Male)
      .WithNationalIdentificationNumber(
        Generate.AnyNationalIdentificationNumberFor(Gender.Male, _dateOfBirth));
  }

  private static LocalDate DateOfBirthWithMinAgeOf(int years)
  {
    var currentDate = Clocks.ZonedUtc.GetCurrentDate();
    var minDate = currentDate.PlusYears(-years);
    var daysInRange = (currentDate - minDate).Days;
    var daysSinceMinBirthDate = Generate.Random.Next(0, daysInRange);
    return minDate + Period.FromDays(daysSinceMinBirthDate);

  }

  public CustomerBuilder WithGuid(Guid guid)
  {
    _guid = guid;
    return this;
  }

  public CustomerBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public CustomerBuilder WithSurname(string surname)
  {
    _surname = surname;
    return this;
  }

  public CustomerBuilder WithDateOfBirth(LocalDate dateOfBirth)
  {
    _dateOfBirth = dateOfBirth;
    return this;
  }

  public CustomerBuilder WithDateOfBirth(int year, int month, int day)
  {
    _dateOfBirth = new LocalDate(year, month, day);
    return this;
  }

  public CustomerBuilder WithGender(Gender gender)
  {
    _gender = gender;
    return this;
  }

  public CustomerBuilder WithNationalIdentificationNumber(string nationalIdentificationNumber)
  {
    _nationalIdentificationNumber = nationalIdentificationNumber;
    return this;
  }

  public CustomerBuilder WithStatus(Status status)
  {
    _status = status;
    return this;
  }

  public Customer Build()
  {
    var customer = new Customer(
      _guid,
      new Person(
        _name,
        _surname,
        _dateOfBirth.Just(),
        _gender,
        _nationalIdentificationNumber));

    if (_status == Status.Student)
    {
      customer.Student();
    }

    return customer;
  }
}