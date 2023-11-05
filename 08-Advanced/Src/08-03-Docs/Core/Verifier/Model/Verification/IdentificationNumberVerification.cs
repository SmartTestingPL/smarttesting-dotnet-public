using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Maybe;
using Core.Verifier.Application;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po PESELu.
/// </summary>
public class IdentificationNumberVerification : IVerification
{
  private readonly IEventEmitter _eventEmitter;
  private readonly ILogger<IdentificationNumberVerification> _logger;

  public IdentificationNumberVerification(
    IEventEmitter eventEmitter, 
    ILogger<IdentificationNumberVerification> logger)
  {
    _eventEmitter = eventEmitter;
    _logger = logger;
  }

  public async Task<VerificationResult> Passes(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running id verification");
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);
    _logger.LogInformation("Id verification done");

    var result = GenderMatchesIdentificationNumber(person)
                 && IdentificationNumberStartsWithDateOfBirth(person)
                 && IdentificationNumberWeightIsCorrect(person);
    _eventEmitter.Emit(new VerificationEvent(this, result));
    return new VerificationResult("id", result);
  }

  private bool GenderMatchesIdentificationNumber(Person person)
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