using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using WebApplication.Verifier.Model;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WebApplicationTests.Verifier;

/// <summary>
/// Przykład użycia ArchTest by upewnić się, że w części naszej aplikacji, gdzie podpinamy
/// framework, klasy z jednej przestrzeni nazw nie są używane w innej.
/// </summary>
public class _04_ArchitectureTests
{
  private static readonly Architecture Architecture =
    new ArchLoader()
      .LoadAssemblies(typeof(FraudController).Assembly)
      .Build();
    
  /// <summary>
  /// Test weryfikujący, że żadne klasy z przestrzeni nazw Verifier.Model nie zależą
  /// od klas z pakietu Verifier.Infrastructure
  /// </summary>
  [Test]
  public static void ShouldNotContainAnyInfrastructureInModelDomain()
  {
    Classes().That().ResideInNamespace("Verifier.Model")
      .Should().NotDependOnAnyTypesThat()
      .ResideInNamespace("Verifier.Infrastructure")
      .Check(Architecture);
  }

}