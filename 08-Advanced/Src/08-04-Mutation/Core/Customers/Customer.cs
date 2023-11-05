﻿using System;
using Newtonsoft.Json;

namespace Core.Customers;

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

  public override string ToString()
  {
    return $"Customer{{Guid={Guid}, Person={Person}}}";
  }
}