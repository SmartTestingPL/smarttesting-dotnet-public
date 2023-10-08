using System;
using System.Globalization;
using E2ETests.Customers;
using NodaTime;

namespace E2ETests.Lib;

public class PlNationalIdentificationNumberProvider
{
  private static readonly int[] PeriodWeights = { 80, 0, 20, 40, 60 };
  private static readonly int[] Weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
  private static readonly int[] SexFields = { 0, 2, 4, 6, 8 };
  private readonly Random _random;

  public PlNationalIdentificationNumberProvider()
  {
    _random = new Random(DateTime.Now.Millisecond);
  }

  public string Generate(LocalDate issueDate, Gender sex)
  {
    var month = CalculateMonth(issueDate.Month, issueDate.Year);
    var day = issueDate.Day;
    var serialNumber = _random.Next(999);
    var sexCode = CalculateSexCode(sex);
    var yearPart = $"{issueDate.ToString("yy", CultureInfo.InvariantCulture)}";
    var monthPart = $"{month:D2}";
    var dayPart = $"{day:D2}";
    var serialNumberPart = $"{serialNumber:D3}";
    var sexCodePart = $"{sexCode}";
    var nationalIdentificationNumber =
      yearPart +
      monthPart +
      dayPart +
      serialNumberPart +
      sexCodePart;
    return nationalIdentificationNumber
           + CalculateChecksum(nationalIdentificationNumber);
  }

  private static int CalculateMonth(int month, int year)
  {
    return month + PeriodWeights[(year - 1800) / 100];
  }

  private int CalculateSexCode(Gender sex)
  {
    return SexFields[_random.Next(SexFields.Length - 1)] + (sex == Gender.Male ? 1 : 0);
  }

  private static int CalculateChecksum(string nationalIdentificationNumber)
  {
    var sum = 0;
    var i = 0;
    var var3 = Weights;
    var var4 = var3.Length;

    for (var var5 = 0; var5 < var4; ++var5)
    {
      var weight = var3[var5];
      var digit = CharUnicodeInfo.GetDigitValue(nationalIdentificationNumber, i++);
      sum += digit * weight;
    }

    var checksum = sum % 10;
    return 0 == checksum ? 0 : 10 - checksum;
  }
}