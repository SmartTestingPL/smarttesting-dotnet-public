using System;
using Core.Scoring.domain;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTests.Scoring.Domain;

public class PeselTests
{
  [Test]
  public void ShouldCreateNewPesel()
  {
    var peselString = "91121345678";

    var pesel = new Pesel(peselString);

    pesel.Value.Should().Be(peselString);
  }

  [Description("Should throw exception when PESEL String = {0}")]
  [TestCase("9112134567")]
  [TestCase("911213456789")]
  public void ShouldThrowExceptionIfPeselLengthNotEqualToEleven(string peselString)
  {
    this.Invoking(_ => new Pesel(peselString))
      .Should().ThrowExactly<ArgumentException>();
  }
}
