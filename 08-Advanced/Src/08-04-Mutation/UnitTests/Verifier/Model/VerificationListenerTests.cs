using Core.Verifier.Application;
using Core.Verifier.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace UnitTests.Verifier.Model;

public class VerificationListenerTests
{
  [Test]
  public void ShouldAddEvent()
  {
    var listener = new VerificationListener(
      NullLogger<VerificationListener>.Instance);
    var @event = new VerificationEvent(this, "age", true);

    listener.Listen(@event);

    listener.Events.Should().Equal(@event);
  }

}