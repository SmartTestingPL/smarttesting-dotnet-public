using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace BikServiceTests;

// TODO: Dotyczy lekcji 05-02
public class ArchitectureTests
{
  private static readonly Architecture Architecture =
    new ArchLoader().LoadAssemblies(
        typeof(Program).Assembly)
      .Build();

  [Test]
  [Ignore("Kod produkcyjny jest zle napisany - ten test ma to wychwycić")]
  public void ShouldNotContainAnyAspNetCoreReferenceInCoreDomain()
  {
    Classes().That().DoNotResideInNamespace("BikService.Infrastructure")
      .Should().NotDependOnAnyTypesThat()
      .ResideInNamespace("BikService.Infrastructure")
      .Check(Architecture);
  }
}
