using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using Core.Scoring.domain;
using NUnit.Framework;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace CoreTests;

public class ArchitectureTests
{
  private static readonly Architecture Architecture =
    new ArchLoader().LoadAssemblies(
        typeof(Pesel).Assembly)
      .Build();

  [Test]
  public void ShouldNotContainAnyAspNetCoreReferenceInCoreDomain()
  {
    Classes().That().ResideInAssembly(
        typeof(Pesel).Assembly)
      .Should().NotDependOnAnyTypesThat()
      .ResideInNamespace("Microsoft.AspNetCore")
      .Check(Architecture);
  }
}


