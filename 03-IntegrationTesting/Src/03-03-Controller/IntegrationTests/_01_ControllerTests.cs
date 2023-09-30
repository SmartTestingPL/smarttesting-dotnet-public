using System;
using FluentAssertions;
using Core.Maybe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using WebApplication;
using WebApplication.Client;
using WebApplication.Controllers;
using WebApplication.Lib;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests;

/// <summary>
/// Klasa testowa do slajdu z testowaniem kontrolera jako obiektu.
/// 
/// Jeśli zainicjujemy kontroler jako obiekt oraz jego zależności to z punktu widzenia kontrolera
/// mamy nic innego jak test jednostkowy. W taki sposób testujemy bez warstwy HTTP
/// logikę naszych komponentów. Zakładając, że przetestowaliśmy jednostkowo
/// `customerVerifier`, taki test nam nic nie daje.
///
/// Zatem skoro naszym celem jest zweryfikowanie czy nasz kontroler komunikuje
/// się po warstwie HTTP to kompletnie nam się to nie udało.
///
/// Czy jest to zły test? Nie, ale trzeba włączyć w to testowanie warstwy HTTP.
/// </summary>
public class _01_ControllerTests
{
  /// <summary>
  /// możemy wykorzystać Asp.Netowy Startup naszej aplikacji, żeby skonfigurował
  /// nam naszą własną instancję kontenera. Kontener zaspokoi zależności
  /// kontrolera i przejdziemy przez wszystkie warstwy
  /// controller -> verifier -> verification.
  /// </summary>
  [Test]
  public void ShouldRejectLoanApplicationWhenPersonTooYoung()
  {
    using var testConfig = new ControllerTestConfig();
    var fraudController = testConfig.Resolve<FraudController>();

    var actionResult = fraudController.FraudCheck(TooYoungZbigniew());

    actionResult.Should().BeOfType<UnauthorizedResult>();
  }

  private static Person TooYoungZbigniew()
  {
    return new Person(
      Any.String(),
      Any.String(),
      (Clocks.ZonedUtc.GetCurrentDate() - 1.Years()).Just(),
      Gender.Male,
      Any.String(),
      Any.Guid());
  }
}

/// <summary>
/// Klasa używająca produkcyjnego Startupu
/// w celu wypełnienia naszej własnej instancji kontenera informacjami
/// o zależnościach.
/// </summary>
public class ControllerTestConfig : IDisposable
{
  private readonly ServiceProvider _container;

  public ControllerTestConfig()
  {
    var containerInstance = new ServiceCollection();
    var startup = new Startup(Any.Instance<IConfiguration>());
    startup.ConfigureServices(containerInstance);
    _container = containerInstance.BuildServiceProvider();
  }

  public T Resolve<T>() where T : notnull
  {
    return _container.GetRequiredService<T>();
  }

  public void Dispose()
  {
    _container.Dispose();
  }
}