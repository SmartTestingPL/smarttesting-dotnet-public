using System;
using Core.Maybe;
using NodaTime;
using ProductionCode.Customers;

namespace UnitTests.Customers;

public class CustomerTestBase
{
  internal static CustomerBuilder Builder()
  {
    return new CustomerBuilder();
  }

  internal class CustomerBuilder
  {
    private LocalDate _dateOfBirth = new LocalDate(1978, 9, 12);
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
          _gender.Just(),
          _nationalIdentificationNumber));

      if (_status == Status.Student)
      {
        customer.Student();
      }

      return customer;
    }
  }
}