using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using FraudDetection.Customers;
using NUnit.Framework;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace FraudDetectionTests.Verifier;

// Dotyczy lekcji 05-02
public class ArchitectureTests
{
  private static readonly Architecture Architecture =
    new ArchLoader().LoadAssemblies(
        typeof(ArchitectureTests).Assembly,
        typeof(Customer).Assembly)
      .Build();

  [Test]
  public void ShouldNotContainAnyAspNetCoreReferenceInVerifications()
  {
    Classes().That().ResideInNamespace("FraudDetection.Verifier.Verification")
      .And().HaveNameEndingWith("Verification")
      .Should().NotDependOnAnyTypesThat()
      .ResideInNamespace("Microsoft.AspNetCore")
      .Check(Architecture);
  }
}