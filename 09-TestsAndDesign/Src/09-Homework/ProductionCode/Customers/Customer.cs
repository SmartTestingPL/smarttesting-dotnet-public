using System;

namespace ProductionCode.Customers;

public class Customer
{
  public Customer(Guid guid, Person person)
  {
    Guid = guid;
    Person = person;
  }

  public Guid Guid { get; }
  public Person Person { get; }
  public bool IsStudent => Person.IsStudent;

  public void Student()
  {
    Person.Student();
  }
}