using FluentAssertions;
using NUnit.Framework;

namespace WebApplicationTests.Verifier.Tdd;

public class _04_FraudVerifierFailingTests
{
  [Test]
  [Ignore("homework")]
  public void ShouldReturnFraudWhenClientHasDebt()
  {
    var verifier = new FraudVerifier();

    var result = verifier.Verify(ClientWithDebt());

    result.Should().NotBeNull("expected to at least get something");
    result.Status.Should().Be(VerificationStatus.Fraud);
  }

  [Test]
  [Ignore("homework")]
  public void ShouldReturnNotFraudWhenClientHasNoDebt()
  {
    var verifier = new FraudVerifier();

    var result = verifier.Verify(ClientWithoutDebt());

    result.Should().NotBeNull("expected to at least get something");
    result.Status.Should().Be(VerificationStatus.NotFraud);
  }

  private class FraudVerifier
  {
    internal VerificationResult Verify(Client client)
    {
      return null;
    }
  }

  private static Client ClientWithDebt()
  {
    return new Client(true);
  }

  private static Client ClientWithoutDebt()
  {
    return new Client(false);
  }

  private class Client
  {
    private readonly bool HasDebt;

    public Client(bool hasDebt)
    {
      HasDebt = hasDebt;
    }
  }

  internal class VerificationResult
  {
    public readonly VerificationStatus Status;

    internal VerificationResult(VerificationStatus status)
    {
      Status = status;
    }
  }

  internal enum VerificationStatus
  {
    Fraud,
    NotFraud
  }
}