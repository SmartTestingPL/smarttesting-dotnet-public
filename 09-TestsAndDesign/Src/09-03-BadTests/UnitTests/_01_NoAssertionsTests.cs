using FluentAssertions;
using NUnit.Framework;

namespace UnitTests;

public class _01_NoAssertionsTests
{
  /// <summary>
  /// Test bez asercji.
  /// </summary>
  [Test]
  public void ShouldReturnSumWhenAddingTwoNumbers()
  {
    const int firstNumber = 1;
    const int secondNumber = 2;

    const int result = firstNumber + secondNumber;

    ThenTwoNumbersShouldBeAdded(result);
  }

  private void ThenTwoNumbersShouldBeAdded(int result)
  {
    // brakuje asercji!!
    result.Should(); //"should be equal to 3"
  }

  /// <summary>
  /// Poprawiony test składający się z samej asercji.
  /// </summary>
  [Test]
  public void ShouldReturnSumWhenAddingTwoNumbersCorrect()
  {
    (1 + 2).Should().Be(3);
  }
}