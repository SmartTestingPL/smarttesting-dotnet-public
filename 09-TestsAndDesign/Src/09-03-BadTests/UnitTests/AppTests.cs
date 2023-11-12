using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Core.NullableReferenceTypesExtensions;
using NUnit.Framework;

namespace UnitTests;

/// <summary>
/// Javowa wersja tego przykładu wykorzystuje klasę biblioteczną
/// StringUtils. C# nie ma klasy StringUtils, ale ją zasymuluję,
/// bo dzięki temu będę w stanie "udawać" mockowanie prywatnych metod,
/// co w wersji Javowej można robić narzędziem PowerMock (W C# z tego co
/// mi wiadomo potrzeba do tego płatnych narzędzi - są dwa otwarte projekty,
/// ale żadnego z nich nie udało mi się doprowadzić do działania).
/// </summary>
public static class StringUtils
{
  // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
  /// <summary>
  /// Zamiast funkcji mam własność trzymającą delegata.
  /// Jak zobaczysz niżej, kod wywołujący takiego delegata
  /// do złudzenia przypomina kod wołający metodę na obiekcie.
  /// </summary>
  public static Func<string?, bool> 
    IsEmpty { get; private set; } = string.IsNullOrEmpty;
}

public class AppTests
{
  /// <summary>
  /// W tym teście mockujemy wszystko co się da. Włącznie z mockowaniem list.
  /// Mockujemy też wywołanie metody z klasy narzędziowej - StringUtils.
  /// </summary>
  [Test]
  public void ShouldFindAnyEmptyName()
  {
    var names = Substitute.For<IList<string?>>();
    var enumerator = Substitute.For<IEnumerator<string>>();
    names.GetEnumerator().Returns(enumerator);
    enumerator.MoveNext().Returns(true, false);
    enumerator.Current.Returns("");
    Replace(typeof(StringUtils), "IsEmpty", s => true, () =>
    {
      new _03_FraudService().AnyNameIsEmpty(names).Should().BeTrue();
    });
  }

  /// <summary>
  /// Poprawiona wersja testu powyżej.
  /// - Nie mockujemy listy - tworzymy ją.
  /// - Nie mockujemy wywołań metody statycznej z StringUtils
  /// (w Javie była to klasa z biblioteki zewnętrznej).
  /// </summary>
  [Test]
  public void ShouldFindAnyEmptyNameFixed()
  {
    var names = new List<string?> {"non empty", ""};

    new _03_FraudService().AnyNameIsEmpty(names).Should().BeTrue();
  }

  /// <summary>
  /// Przykład opakowania kodu, wywołującego metodę statyczną
  /// łączącą się do bazy danych.
  /// </summary>
  [Test]
  public void ShouldDoSomeWorkInDatabaseWhenEmptyStringFound()
  {
    var wrapper = Substitute.For<IDatabase>();
    var names = new List<string> {"non empty", ""};

    new FraudServiceFixed(wrapper).AnyNameIsEmpty(names);

    wrapper.Received(1).StoreInDatabase();
  }

  /// <summary>
  /// Udawanie mockowania prywatnych metod. Tak naprawdę ta funkcja
  /// podmienia statycznego delegata, który jest przypisany do własności
  /// - taki hack na potrzeby tego przykładu.
  ///
  /// Wykonuje przekazaną akcję, po czym przywraca starą wartość
  /// delegata.
  /// </summary>
  /// <param name="type">Typ w którym podmieniamy delegata</param>
  /// <param name="propertyName">Własność która trzyma delegata</param>
  /// <param name="value">wartość na</param>
  /// <param name="action">akcja do wykonania podczas
  /// podmienienia delegata</param>
  private static void Replace(
    Type type, 
    string propertyName, 
    Func<string, bool> value, 
    Action action)
  {
    var property = type
      .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)
      .OrThrow();
    var originalValue = property.GetValue(null);
    property.SetValue(null, value);
    try
    {
      action();
    }
    finally
    {
      property.SetValue(null, originalValue);
    }
  }
}

/// <summary>
/// Klasa, w której wołamy metody statyczne.
/// </summary>
internal class _03_FraudService
{
  internal bool AnyNameIsEmpty(IList<string?> names)
  {
    foreach (var name in names)
    {
      if (StringUtils.IsEmpty(name))
      {
        DatabaseAccessor.StoreInDatabase();
        return true;
      }
    }

    return false;
  }
}

/// <summary>
/// Poprawiona implementacja, gdzie zamiast wywołania statycznego
/// <see cref="DatabaseAccessor"/>
/// wywołujemy naszą wersję <see cref="IDatabase"/>.
/// </summary>
internal class FraudServiceFixed
{
  private readonly IDatabase _database;

  internal FraudServiceFixed(IDatabase database)
  {
    _database = database;
  }

  internal bool AnyNameIsEmpty(IEnumerable<string> names)
  {
    foreach (var name in names)
    {
      if (StringUtils.IsEmpty(name))
      {
        _database.StoreInDatabase();
        return true;
      }
    }

    return false;
  }
}

/// <summary>
/// Interfejs do klasy opakowującej wywołanie metody statycznej
/// </summary>
public interface IDatabase
{
  void StoreInDatabase();
}

/// <summary>
/// Nasza klasa opakowująca wywołanie metody statycznej.
/// </summary>
public class DatabaseAccessorWrapper : IDatabase
{
  public virtual void StoreInDatabase()
  {
    DatabaseAccessor.StoreInDatabase();
  }
}

/// <summary>
/// Klasa symulująca dostęp do bazy danych.
/// </summary>
internal class DatabaseAccessor
{
  public static void StoreInDatabase()
  {

  }
}