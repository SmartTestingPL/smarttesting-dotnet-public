using System;
using System.Text.Json.Serialization;

namespace FraudVerifier.Customers;

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