using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using Core.Customers;
using NUnit.Framework;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace CoreTests;

/// <summary>
/// Przykład użycia ArchUnitNeta by upewnić się, że w głównej części
/// naszej dziedziny nie mamy klas związanych z frameworkiem
/// (np. Asp.Net Core).
///
/// Inną biblioteką w .NET której możemy użyć jest NetArchTest,
/// ale wybrałem ArchUnitNeta z powodu bliższego podobieństwa
/// do ArchUnita użytego w Javowym przykładzie.
/// </summary>
public class _03_ArchitectureTests
{
  private static readonly Architecture Architecture =
    new ArchLoader().LoadAssemblies(
        typeof(Customer).Assembly)
      .Build();

  /// <summary>
  /// Test weryfikujący, że żadna klasa z projektu Core i jego testów
  /// nie zależy od klasy z pakietu frameworka Asp.Net Core.
  /// </summary>
  [Test]
  public void ShouldNotContainAnyAspNetCoreReferenceInCoreDomain()
  {
    Classes().That().ResideInAssembly(
        typeof(_03_ArchitectureTests).Assembly,
        typeof(Customer).Assembly)
      .Should().NotDependOnAnyTypesThat()
      .ResideInNamespace("Microsoft.AspNetCore")
      .Check(Architecture);
  }
}