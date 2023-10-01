using System;
using System.Collections.Generic;
using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProductionCode.Client;
using ProductionCode.Lib;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests.Client;

/// <summary>
/// Narzędzie do IOC / DI, mogą posiadać integrację z narzędziami do testów
/// NUnit nie ma wbudowanej takiej integracji, ale jeśli się uprzemy, możemy ją łatwo
/// odtworzyć w sposób "chałupniczy". Tutaj widzimy wykorzystanie sprzęgnięcie NUnita
/// z kontenerem poprzez test parametryzowany.
/// </summary>
public class _05_DependencyInjectionTestArgument
{
  /// <summary>
  /// Test bierze swoje parametry z metody IoCContainer,
  /// przekazując jej typ CustomerVerifier jako argument
  /// </summary>
  /// <param name="verifier">parametr wyciągnięty z kontenera w metodzie IocContainer</param>
  [TestCaseSource(nameof(IoCContainer), new object[] { typeof(CustomerVerifier) })]
  public void ShouldPassVerificationWhenNonFraudGetsVerified(CustomerVerifier verifier)
  {
    //wywołanie logiki biznesowej
    var result = verifier.Verify(TooYoungStefan());

    result.Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  /// <summary>
  /// Metoda służąca jako źródło danych dla testu.
  /// Wyciąga pożądane obiekty z kontenera.
  /// Nie róbcie tego w domu. <see cref="TestCaseSourceAttribute"/> wymaga
  /// by ta metoda była statyczna, więc albo będziemy (tak jak tutaj)
  /// tworzyć kontener za każdym razem i nie czyścić go metodą Dispose(),
  /// albo będziemy musieli zdefiniować w klasie statyczny kontener,
  /// czyszczony dopiero w <see cref="OneTimeTearDownAttribute"/>, co
  /// upośledzi izolację testów!
  /// </summary>
  /// <param name="dependencyType">typ zależności</param>
  /// <returns>zależność jako tablicę obiektów (tego wymaga NUnit)</returns>
  public static IEnumerable<object> IoCContainer(Type dependencyType)
  {
    yield return _02_Config.CreateContainerInstance().GetRequiredService(dependencyType);
  }

  private static Person TooYoungStefan()
  {
    return new Person(
      Any.String(),
      Any.String(),
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      Any.String(),
      Any.Guid());
  }
}