using System;
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