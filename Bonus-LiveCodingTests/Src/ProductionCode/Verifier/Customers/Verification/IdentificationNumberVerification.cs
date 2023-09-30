using System;
using System.Globalization;
using Functional.Maybe;
using ProductionCode.Customers;
using ProductionCode.Events;

namespace ProductionCode.Verifier.Customers.Verification;

/// <summary>
/// Weryfikacja poprawności numeru PESEL.
/// Zob. https://pl.wikipedia.org/wiki/PESEL#Cyfra_kontrolna_i_sprawdzanie_poprawno.C5.9Bci_numeru
/// </summary>
public class IdentificationNumberVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;

  public IdentificationNumberVerification(IEventEmitter eventEmitter)
  {
    _eventEmitter = eventEmitter;
  }

  public bool Passes(Person person)
  {
    var passes = GenderMatchesIdentificationNumber(person)
                 && IdentificationNumberStartsWithDateOfBirth(person)
                 && IdentificationNumberWeightIsCorrect(person);
    _eventEmitter.Emit(new VerificationEvent(passes));
    return passes;
  }

  private static bool GenderMatchesIdentificationNumber(Person person)
  {
    if (int.Parse(person.NationalIdentificationNumber.Substring(9, 1)) % 2 == 0)
    {
      return person.Gender == Gender.Female;
    }

    return person.Gender == Gender.Male;
  }

  private bool IdentificationNumberStartsWithDateOfBirth(Person person)
  {
    var dateOfBirthString = person.DateOfBirth
      .Select(date => date.ToString("yyMMdd", new DateTimeFormatInfo()))
      .OrElse(() => new InvalidOperationException("Need date of birth filled to proceed."));

    if (dateOfBirthString[0] == '0')
    {
      var s = dateOfBirthString[2..4];
      var monthNum = int.Parse(s);
      monthNum += 20;
      dateOfBirthString = dateOfBirthString[..2] + monthNum + dateOfBirthString[4..6];
    }

    return dateOfBirthString == person.NationalIdentificationNumber[..6];
  }

  private bool IdentificationNumberWeightIsCorrect(Person person)
  {
    var weights = new[] { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

    if (person.NationalIdentificationNumber.Length != 11)
    {
      return false;
    }

    var weightSum = 0;
    for (var i = 0; i < 10; i++)
    {
      weightSum += int.Parse(person.NationalIdentificationNumber[i..(i + 1)]) * weights[i];
    }

    var actualSum = (10 - weightSum % 10) % 10;

    var checkSum = int.Parse(person.NationalIdentificationNumber[10..11]);

    return actualSum == checkSum;
  }
}