using FluentAssertions;
using NUnit.Framework;

namespace WebApplicationTests.Verifier.Tdd;

/// <summary>
/// Kod ze slajdów [Zacznijmy od testu].
/// </summary>
public class _01_AcceptanceTests
{
  [Test]
  [Ignore("homework")]
  public void ShouldVerifyAClientWithDebtAsFraud()
  {
    var fraud = ClientWithDebt();

    var verification = VerifyClient(fraud);

    ThenIsVerifiedAsFraud(verification);
  }

  private object ClientWithDebt()
  {
    return null;
  }

  private object VerifyClient(object client)
  {
    return null;
  }

  private void ThenIsVerifiedAsFraud(object verification)
  {
    verification.Should().NotBeNull();
  }
}