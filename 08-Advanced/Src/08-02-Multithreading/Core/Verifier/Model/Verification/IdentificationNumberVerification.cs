using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Verifier.Application;
using Core.Maybe;
using Microsoft.Extensions.Logging;

namespace Core.Verifier.Model.Verification;

/// <summary>
/// Weryfikacja po numerze PESEL.
/// Po zakończonym procesowaniu weryfikacji wysyła zdarzenie z rezultatem weryfikacji.
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

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na zadaniach (taskach).
  /// </summary>
  public async Task<VerificationResult> PassesAsync(Person person, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Running id verification");
    // Symuluje procesowanie w losowym czasie do 2 sekund
    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(2000)), cancellationToken);
    _logger.LogInformation("Id verification done");

    var result = GenderMatchesIdentificationNumber(person)
                 && IdentificationNumberStartsWithDateOfBirth(person)
                 && IdentificationNumberWeightIsCorrect(person);
    _eventEmitter.Emit(new VerificationEvent(this, "id", result));
    return new VerificationResult("id", result);
  }

  /// <summary>
  /// W Javowym odpowiedniku jest tylko jedna metoda passes(). Tu są dwie.
  /// Jedne przykłady używają jednej, a inne tej drugiej.
  /// Ta wersja metody działa na zwykłych wątkach.
  /// </summary>
  public VerificationResult Passes(Person person)
  {
    _logger.LogInformation("Running id verification");
    // Symuluje procesowanie w losowym czasie do 2 sekund
    Thread.Sleep(new Random().Next(2000));
    _logger.LogInformation("Id verification done");

    var result = GenderMatchesIdentificationNumber(person)
                 && IdentificationNumberStartsWithDateOfBirth(person)
                 && IdentificationNumberWeightIsCorrect(person);
    _eventEmitter.Emit(new VerificationEvent(this, "id", result));
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

  private static bool IdentificationNumberStartsWithDateOfBirth(Person person)
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