using System.Collections.Generic;
using FluentAssertions;
using Core.Maybe;
using NUnit.Framework;
using ProductionCode.Client;
using ProductionCode.Lib;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Strings;
using static TddXt.AnyRoot.Root;

namespace IntegrationTests.Client;

public class _01_IocTests
{
  /// <summary>
  /// Kod przedstawiony na slajdzie [W jaki sposób tworzysz obiekty?].
  /// Przedstawiamy tu ręczne utworzenie drzewa zależności obiektów.
  /// Trzeba pamiętać o odpowiedniej kolejności utworzenia obiektów oraz
  /// w jednym miejscu mieszamy tworzenie i realizacje akcji biznesowej
  /// wywołanie:
  /// 
  /// new CustomerVerifier(...).Verify(...);
  /// </summary>
  [Test]
  public void ManualObjectGeneration()
  {
    // Tworzenie Age Verification
    var httpCallMaker = new HttpCallMaker();
    var accessor = new DatabaseAccessor();
    var ageVerification = new AgeVerification(httpCallMaker, accessor);

    // Tworzenie ID Verification
    var idVerification = new IdentificationNumberVerification(accessor);

    // Tworzenie Name Verification
    var eventEmitter = new EventEmitter();
    var nameVerification = new NameVerification(eventEmitter);

    // Wywołanie logiki biznesowej
    var result = new CustomerVerifier(
      new List<IVerification>
      {
        ageVerification,
        idVerification,
        nameVerification
      }
    ).Verify(Stefan());

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