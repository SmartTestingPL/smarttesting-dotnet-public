using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests;

public class _02_DoesNSubstituteWorkTests
{
  /// <summary>
  /// W tym teście de facto weryfikujemy czy framework do mockowania działa.
  /// </summary>
  [Test]
  public void ShouldReturnPositiveFraudVerificationWhenFraud()
  {
    var service = Substitute.For<IAnotherFraudService>();
    service.IsFraud(Arg.Any<Person>()).Returns(true);

    new FraudService(service).CheckIfFraud(new Person()).Should().BeTrue();
  }
}

internal class FraudService
{
  private readonly IAnotherFraudService _service;

  internal FraudService(IAnotherFraudService service)
  {
    _service = service;
  }

  internal bool CheckIfFraud(Person person)
  {
    return _service.IsFraud(person);
  }
}

public interface IAnotherFraudService
{
  bool IsFraud(Person person);
}

public class Person
{

}