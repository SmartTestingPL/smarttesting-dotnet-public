using System.Collections.Generic;
using System.Linq;

namespace ProductionCode.Client;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i jeśli przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(IReadOnlyCollection<IVerification> verifications)
  {
    _verifications = verifications;
  }

  /// <summary>
  /// Główna metoda biznesowa. Weryfikuje czy dana osoba jest oszustem.
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public CustomerVerificationResult Verify(Person person)
  {
    if (_verifications.All(verification => verification.Passes()))
    {
      return CustomerVerificationResult.Passed(person.Guid);
    }

    return CustomerVerificationResult.Failed(person.Guid);
  }
}

/// <summary>
/// Weryfikacja po wieku.
/// 
/// Na potrzeby scenariusza lekcji, brak prawdziwej implementacji.
/// Klasa symuluje połączenie po HTTP i po bazie danych.
/// </summary>
public class AgeVerification : IVerification
{
  private readonly IHttpCallMaker _maker;
  private readonly IDatabaseAccessor _accessor;

  public AgeVerification(IHttpCallMaker maker, IDatabaseAccessor accessor)
  {
    _maker = maker;
    _accessor = accessor;
  }
}

/// <summary>
/// Weryfikacja po numerze pesel.
/// 
/// Na potrzeby scenariusza lekcji, brak prawdziwej implementacji.
/// Klasa symuluje połączenie po bazie danych.
/// </summary>
public class IdentificationNumberVerification : IVerification
{
  private readonly IDatabaseAccessor _accessor;

  public IdentificationNumberVerification(IDatabaseAccessor accessor)
  {
    _accessor = accessor;
  }
}

/// <summary>
/// Weryfikacja po nazwisku.
/// 
/// Na potrzeby scenariusza lekcji, brak prawdziwej implementacji.
/// Klasa symuluje połączenie po brokerze wiadomości.
/// </summary>
public class NameVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;

  public NameVerification(IEventEmitter eventEmitter)
  {
    _eventEmitter = eventEmitter;
  }
}

/// <summary>
/// Interfejs klasy udającej klasę łączącą się po HTTP.
/// </summary>
public interface IHttpCallMaker
{
}

/// <summary>
/// klasa udająca klasę łączącą się po HTTP.
/// </summary>
public class HttpCallMaker : IHttpCallMaker
{
}

/// <summary>
/// Interfejs klasy udającej klasę łączącą się po bazie danych.
/// </summary>
public interface IDatabaseAccessor
{
}

/// <summary>
/// Klasa udająca klasę łączącą się po bazie danych.
/// </summary>
public class DatabaseAccessor : IDatabaseAccessor
{
}

/// <summary>
/// Interfejs klasy udającej klasę łączącą się po brokerze wiadomości.
/// </summary>
public interface IEventEmitter
{
}

/// <summary>
/// Klasa udająca klasę łączącą się po brokerze wiadomości.
/// </summary>
public class EventEmitter : IEventEmitter
{
}