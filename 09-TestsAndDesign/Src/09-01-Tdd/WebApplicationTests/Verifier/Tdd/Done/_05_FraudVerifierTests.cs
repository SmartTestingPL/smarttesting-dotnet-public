using FluentAssertions;
using NUnit.Framework;
using WebApplication.Controllers;
using WebApplication.Logic;

namespace WebApplicationTests.Verifier.Tdd.Done;

public class _05_FraudVerifierTests
{
  [Test]
  public void ShouldReturnFraudWhenClientHasDebt()
  {
    var verifier = new DoneFraudVerifier();

    var result = verifier.Verify(ClientWithDebt());

    result.Should().NotBeNull();
    result.Status.Should().Be(VerificationStatus.Fraud);
  }

  [Test]
  public void ShouldReturnNotFraudWhenClientHasNoDebt()
  {
    var verifier = new DoneFraudVerifier();

    var result = verifier.Verify(ClientWithoutDebt());

    result.Should().NotBeNull();
    result.Status.Should().Be(VerificationStatus.NotFraud);
  }

  private static Client ClientWithDebt()
  {
    return new Client(true);
  }

  private static Client ClientWithoutDebt()
  {
    return new Client(false);
  }
}