using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Functional.Maybe.Just;
using NodaTime;
using NodaTime.Text;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Verifier;
using ProductionCode.Verifier.Customers.Verification;
using static TddXt.AnyRoot.Root;

namespace UnitTests.Verifier.Customers.Verification;

/// <summary>
/// Klasa zawiera przykłady testów parametryzowanych.
///
/// W NUnicie można to osiągnąć jeszcze na kilka innych sposobów
/// niż te pokazane poniżej, w zależności od tego jak zaawansowany
/// scenariusz chcemy obsłużyć. 
/// </summary>
public class NationalIdentificationNumberVerificationTest
{
  private static readonly object[] IdVerificationArgumentsProvider =
  {
    new object[] {new LocalDate(1998, 3, 14), Gender.Female, true},
    new object[] {new LocalDate(1998, 3, 14), Gender.Male, false},
    new object[] {new LocalDate(2000, 3, 14), Gender.Female, false}
  };

  [Test]
  public void VerificationShouldPassForCorrectIdentificationNumber()
  {
    // given
    var person = BuildPerson(new LocalDate(1998, 3, 14), Gender.Female);
    var verification = new IdentificationNumberVerification(Any.Instance<EventEmitter>());

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeTrue();
  }

  [Test]
  public void VerificationShouldFailForInconsistentGender()
  {
    // given
    var person = BuildPerson(new LocalDate(1998, 3, 14), Gender.Male);
    var verification = new IdentificationNumberVerification(Any.Instance<EventEmitter>());

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  [Test]
  public void ShouldReturnFalseForWrongYearOfBirth()
  {
    //given
    var person = BuildPerson(new LocalDate(2000, 3, 14), Gender.Female);
    var verification = new IdentificationNumberVerification(Any.Instance<EventEmitter>());

    // when
    var passes = verification.Passes(person);

    // then
    passes.Should().BeFalse();
  }

  // Test tych samych przypadków co w 3 różnych testach powyżej przy pomocy
  //  parametrów zwracanych przez metodę.
  //
  // w oryginalnym przykładzie JUnitowym opis testu był również parametryzowany.
  // nie znalazłem odpowiedniej funkcjonalności wbudowanej w NUnita.
  [Description("should return result for birth date and gender")]
  [TestCaseSource(nameof(IdVerificationArgumentsProvider))]
  public void ShouldVerifyNationalIdentificationNumberAgainstPersonalData(
    LocalDate birthDate, Gender gender, bool passes)
  {
    //given
    var person = BuildPerson(birthDate, gender);
    var verification = new IdentificationNumberVerification(Any.Instance<IEventEmitter>());

    // when
    var actualPasses = verification.Passes(person);

    // then
    actualPasses.Should().Be(passes);
  }

  public static IEnumerable<IdVerificationArgument> IdVerificationArgumentsProviderFromCsv(string filePath)
  {
    using var streamReader = new StreamReader(filePath);
    using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
    });
    csv.Context.RegisterClassMap<IdVerificationArgument.Map>();
    
    var idVerificationArguments = csv.GetRecords<IdVerificationArgument>().ToList();
    return idVerificationArguments;
  }

  // Test tych samych przypadków co w 3 różnych testach powyżej przy pomocy
  // parametrów z pliku CSV.
  [Description("should return result for birth date and gender")]
  [TestCaseSource(nameof(IdVerificationArgumentsProviderFromCsv), new object[] { "pesel.csv" })]
  public void ShouldVerifyNationalIdentificationNumberAgainstPersonalDataFromFile(
    IdVerificationArgument arg)
  {
    // given
    var person = BuildPerson(arg.BirthDate, arg.Gender);
    var verification = new IdentificationNumberVerification(
      new EventEmitter());

    // when
    var actualPasses = verification.Passes(person);

    // then
    actualPasses.Should().Be(arg.Passes);
  }

  private static Person BuildPerson(LocalDate birthDate, Gender gender)
  {
    return new Person("John", "Doe", birthDate.Just(), gender, "98031416402");
  }
}

public class IdVerificationArgument
{
  public LocalDate BirthDate { get; set; }
  public Gender Gender { get; set; }

  public bool Passes { get; set; }

  //wartość zwracana przez tę metodę będzie wypisywana w nazwie testu
  //w odpalaczu testów, w miejscu argumentu.
  public override string ToString()
  {
    return $"{nameof(BirthDate)}: {BirthDate}, {nameof(Gender)}: {Gender}, {nameof(Passes)}: {Passes}";
  }

  /// <summary>
  /// Jako że parsowanie typu LocalDate jest nieoczywiste, musimy podać własną logikę mapowania z CSV
  /// </summary>
  public sealed class Map : ClassMap<IdVerificationArgument>
  {
    public Map()
    {
      Map(m => m.BirthDate).Convert(args => LocalDatePattern.Iso.Parse(args.Row["birthDate"]).Value);
      Map(m => m.Gender).Name("gender");
      Map(m => m.Passes).Name("passes");
    }
  }
}
