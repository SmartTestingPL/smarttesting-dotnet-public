using FluentAssertions;
using Core.Maybe;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProductionCode.Client;
using ProductionCode.Lib;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests.Client;

public class _03_AspNetIocTests
{
  /// <summary>
  /// Kod przedstawiony na slajdzie po zdefiniowaniu klasy konfiguracyjnej.
  /// Ten test uruchamia kontener IoC i pozwala pozyskać z niego gotowe obiekty.
  /// Oddzielamy w ten sposób konstrukcję
  /// 
  /// var context = _02_Config.CreateContainerInstance();
  /// var verifier = context.GetRequiredService<CustomerVerifier>();
  /// 
  /// od użycia
  /// 
  /// var result = verifier.Verify(TooYoungStefan());
  /// </summary>
  [Test]
  public void ShouldPassVerificationWhenNonFraudGetsVerified()
  {
    // Utworzenie gotowego kontenera
    using var context = _02_Config.CreateContainerInstance();

    // Wyciągnięcie obiektu z kontenera
    var verifier = context.GetRequiredService<CustomerVerifier>();

    // Wywołanie logiki biznesowej
    var result = verifier.Verify(Stefan());

    // Asercja
    result.Status.Should().Be(VerificationStatus.VerificationFailed);
  }

  private static Person Stefan()
  {
    return new Person(
      Any.String(),
      Any.String(),
      Clocks.ZonedUtc.GetCurrentDate().Just(),
      Gender.Male,
      Any.String(),
      Any.Guid());
  }
}