using System;
using Newtonsoft.Json;

namespace ProductionCode.Customers;

/// <summary>
/// Klient. Klasa opakowująca osobę do zweryfikowania.
/// </summary>
public class Customer
{
  public Customer(Guid guid, Person person)
  {
    Guid = guid;
    Person = person;
  }

  public Guid Guid { get; }
  public Person Person { get; }

  [JsonIgnore]
  public bool IsStudent => Person.IsStudent;

  public void Student()
  {
    Person.Student();
  }
}