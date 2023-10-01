using System;
using Functional.Maybe.Just;
using NodaTime;
using ProductionCode.Customers;

namespace UnitTests.Verifier.Customers;

/// <summary>
/// Klasa bazowa z przykładem buildera obiektu wykorzystywanego w teście.
/// </summary>
public class CustomerTestBase
{
  internal static CustomerBuilder Builder()
  {
    return new CustomerBuilder();
  }

  /// <summary>
  /// Przykład budowniczego do danych testowych. To jest przykład przetłumaczony z Javy.
  /// Ten rodzaj budowniczego jest bardzo elastyczny. Natomiast pod tą klasą zamieściłem
  /// specyficzny dla C# przykład prostszego buildera, który wystarcza mi
  /// w 99% przypadków w testach jednostkowych.
  /// </summary>
  internal class CustomerBuilder
  {
    private LocalDate _dateOfBirth = new(1978, 9, 12);
    private Gender _gender = Gender.Female;
    private Guid _guid = Guid.NewGuid();
    private string _name = "Anna";
    private string _nationalIdentificationNumber = "78091211463";
    private Status _status = Status.NotStudent;
    private string _surname = "Kowalska";

    internal CustomerBuilder WithGuid(Guid guid)
    {
      _guid = guid;
      return this;
    }

    internal CustomerBuilder WithName(string name)
    {
      _name = name;
      return this;
    }

    internal CustomerBuilder WithSurname(string surname)
    {
      _surname = surname;
      return this;
    }

    internal CustomerBuilder WithDateOfBirth(LocalDate dateOfBirth)
    {
      _dateOfBirth = dateOfBirth;
      return this;
    }

    internal CustomerBuilder WithDateOfBirth(int year, int month, int day)
    {
      _dateOfBirth = new LocalDate(year, month, day);
      return this;
    }

    internal CustomerBuilder WithGender(Gender gender)
    {
      _gender = gender;
      return this;
    }

    internal CustomerBuilder WithNationalIdentificationNumber(string nationalIdentificationNumber)
    {
      _nationalIdentificationNumber = nationalIdentificationNumber;
      return this;
    }

    internal CustomerBuilder WithStatus(Status status)
    {
      _status = status;
      return this;
    }

    internal Customer Build()
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
}

/// <summary>
/// Przykład mniej elastycznego, ale prostszego buildera. Można go wywołać z listą inicjalizacyjną,
/// np. new CustomerBuilderSimpleDotNetStyle { Guid = ..., Name = ... }.Build();
/// </summary>
internal record CustomerBuilderSimpleDotNetStyle
{
  internal Guid Guid { private get; set; } = Guid.NewGuid();
  internal string Name { private get; set; } = "Anna";
  internal string Surname { private get; set; } = "Kowalska";
  internal LocalDate DateOfBirth { private get; set; } = new(1978, 9, 12);
  internal (int year, int month, int day) DateOfBirthDMY
  {
    set => DateOfBirth = new LocalDate(value.year, value.month, value.day);
  }
  internal Gender Gender { private get; set; } = Gender.Female;
  internal string NationalIdentificationNumber { private get; set; } = "78091211463";
  internal Status Status { private get; set; } = Status.NotStudent;

  private Customer Build()
  {
    var customer = new Customer(
      Guid,
      new Person(
        Name,
        Surname,
        DateOfBirth.Just(),
        Gender,
        NationalIdentificationNumber));

    if (Status == Status.Student)
    {
      customer.Student();
    }

    return customer;
  }
}