using FluentAssertions;
using NUnit.Framework;
using ProductionCode.ModuleA;

namespace UnitTests.ModuleA;

/// <summary>
/// Test pokazujący testowanie getterów i setterów. Jeśli w tych metodach
/// nie ma specjalnej logiki, najlepiej nie pisać takich testów.
/// </summary>
public class MyPocoTests
{
  [Test]
  public void ShouldTestGettersAndSetters()
  {
    var myPoco = new MyPoco
    {
      Age = 10, 
      Name = "Name", 
      Surname = "Surname"
    };

    myPoco.Age.Should().Be(10);
    myPoco.Name.Should().Be("Name");
    myPoco.Surname.Should().Be("Surname");
  }
}