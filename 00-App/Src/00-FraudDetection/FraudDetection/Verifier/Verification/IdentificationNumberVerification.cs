using System;
using System.Globalization;
using FraudDetection.Customers;
using Core.Maybe;
using Microsoft.Extensions.Logging;

namespace FraudDetection.Verifier.Verification;

/// <summary>
/// Weryfikacja po numerze PESEL. Osoba z odpowiednim numerem PESEL
/// zweryfikowana pozytywnie. Algorytm napisany na podstawie https://obywatel.gov.pl/pl/dokumenty-i-dane-osobowe/czym-jest-numer-pesel.
/// 
/// 
/// Co oznaczaj¹ poszczególne cyfry w numerze PESEL
/// 
/// Ka¿da z 11 cyfr w numerze PESEL ma swoje znaczenie. Mo¿na je podzieliæ nastêpuj¹co:
/// 
/// RRMMDDPPPPK
/// 
/// RR - to 2 ostanie cyfry roku urodzenia,
/// 
/// MM - to miesi¹c urodzenia (zapoznaj siê z sekcj¹  "Dlaczego osoby urodzone po 1999 roku maj¹ inne oznaczenie miesi¹ca urodzenia", która znajduje siê poni¿ej),
/// 
/// DD - to dzieñ urodzenia,
/// 
/// PPPP - to liczba porz¹dkowa oznaczaj¹ca p³eæ. U kobiety ostatnia cyfra tej liczby jest parzysta (0, 2, 4, 6, 8), a u mê¿czyzny - nieparzysta (1, 3, 5, 7, 9),
/// 
/// K - to cyfra kontrolna.
/// 
/// Przyk³ad: PESEL 810203PPP6K nale¿y do kobiety, która urodzi³a siê 3 lutego 1981 roku, a PESEL 761115PPP3K - do mê¿czyzny, który urodzi³ siê 15 listopada 1976 roku.
/// </summary>
public class IdentificationNumberVerification : IVerification
{
  private readonly ILogger<IdentificationNumberVerification> _log;

  public IdentificationNumberVerification(ILogger<IdentificationNumberVerification> log)
  {
    _log = log;
  }

  public bool Passes(Person person)
  {
    var passed = GenderMatchesIdentificationNumber(person)
                 && IdentificationNumberStartsWithDateOfBirth(person)
                 && IdentificationNumberWeightIsCorrect(person);
    _log.LogInformation($"Person [{person}] passed the id number check [{passed}]");
    return passed;
  }

  private static bool GenderMatchesIdentificationNumber(Person person)
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

  /// <summary>
  /// Pomnó¿ ka¿d¹ cyfrê z numeru PESEL przez odpowiedni¹ wagê: 1-3-7-9-1-3-7-9-1-3.
  /// </summary>
  private static bool IdentificationNumberWeightIsCorrect(Person person)
  {
    int[] weights = {1, 2, 7, 8, 1, 3, 7, 9, 1, 3};

    if (person.NationalIdentificationNumber.Length != 11)
    {
      return false;
    }

    // Dodaj do siebie otrzymane wyniki. Uwaga, jeœli w trakcie mno¿enia otrzymasz liczbê dwucyfrow¹,
    // nale¿y dodaæ tylko ostatni¹ cyfrê (na przyk³ad zamiast 63 dodaj 3).
    var weightSum = 0;
    for (var i = 0; i < 10; i++)
    {
      weightSum += int.Parse(person.NationalIdentificationNumber[i..(i + 1)]) * weights[i];
    }

    var actualSum = (10 - weightSum % 10) % 10;

    // Odejmij uzyskany wynik od 10. Uwaga: jeœli w trakcie dodawania otrzymasz liczbê dwucyfrow¹,
    // nale¿y odj¹æ tylko ostatni¹ cyfrê (na przyk³ad zamiast 32 odejmij 2).
    // Cyfra, która uzyskasz, to cyfra kontrolna. 10 - 2 = 8
    var checkSum = int.Parse(person.NationalIdentificationNumber[10..11]);

    return actualSum == checkSum;
  }
}