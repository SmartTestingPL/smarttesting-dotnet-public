using NSubstitute;
using NUnit.Framework;

namespace CoreTests.Lesson1;

/// <summary>
/// Klasa z przykładami źle zaprojektowanego kodu.
/// 
/// Testy nie zawierają asercji, są tylko po to by skopiować
/// je na slajdy z pewnością, że się nadal kompilują.
/// 
/// Klasy implementacyjne poniżej nic nie robią. Chodzi jedynie o
/// zaprezentowanie pewnych koncepcji.
/// </summary>
// ReSharper disable once TestFileNameWarning
public class _01_BadClassTests
{

  /// <summary>
  /// Kod widoczny na slajdzie [Testy a architektura].
  /// W komentarzach zakładamy pewne kwestie po spojrzeniu w
  /// wyimaginowaną implementację (oczywiście na
  /// potrzeby tej lekcji takiej implementacji nie ma -
  /// chodzi nam o pewne koncepcje).
  /// </summary>
  [Test]
  public void ShouldBuildAnObject()
  {
    // imię musi być null, żeby wykonać specjalny scenariusz
    // uruchomienia nie chcemy liczyć dodatkowych opłat ze względu
    // na konto, zatem ustawiamy je na null w kodzie okazuje się,
    // że nikt nie korzysta z usług marketingu więc też ustawiamy na null
    var person = new Person(
      null,
      "Kowalski",
      null,
      new PhoneService(),
      null,
      new TaxService(),
      null);
  }

  /// <summary>
  /// Czy zdarzyło Ci się, że dodawanie kolejnych testów było
  /// dla Ciebie drogą przez mękę? Czy znasz przypadki, gdzie
  /// potrzebne były setki linijek kodu przygotowującego pod
  /// uruchomienie testu? Oznacza to, że najprawdopodobniej
  /// albo nasz sposób testowania jest niepoprawny albo
  /// architektura aplikacji jest zła.
  /// </summary>
  [Test]
  public void ShouldUseALotOfMocks()
  {
    var accountService = Substitute.For<IAccountService>();
    accountService.Calculate().Returns("PL123080123");
    var phoneService = Substitute.For<IPhoneService>();
    phoneService.Calculate().Returns("+4812371237");
    var marketingService = Substitute.For<IMarketingService>();
    marketingService.Calculate().Returns("MAIL");
    var taxService = Substitute.For<ITaxService>();
    taxService.Calculate().Returns("1_000_000PLN");
    var reportingService = Substitute.For<IReportingService>();
    taxService.Calculate().Returns("FAILED");
    var person = new Person(
      "Jan",
      "Kowalski",
      accountService,
      phoneService,
      reportingService)
    {
      MarketingService = marketingService,
      TaxService = taxService
    };
    // nie rozumiem różnicy między Calculate i Count
    person.Calculate();
    person.Count();
    person.Order();
  }
}

class Person
{
  internal string Name;
  internal string SurName;
  internal IAccountService Account;
  internal IPhoneService PhoneService;
  internal IMarketingService MarketingService;
  internal ITaxService TaxService;
  internal IReportingService ReportingService;

  internal Person(
    string name, 
    string surName, 
    IAccountService account,
    IPhoneService phoneService,
    IMarketingService marketingService,
    ITaxService taxService,
    IReportingService reportingService)
  {
    Name = name;
    SurName = surName;
    Account = account;
    PhoneService = phoneService;
    MarketingService = marketingService;
    TaxService = taxService;
    ReportingService = reportingService;
  }

  internal Person(
    string name, string surName, IAccountService account,
    IPhoneService phoneService,
    IReportingService reportingService)
  {
    Name = name;
    SurName = surName;
    Account = account;
    PhoneService = phoneService;
    ReportingService = reportingService;
  }

  internal string Order()
  {
    return "ordered";
  }

  internal string Count()
  {
    return "calculated";
  }

  internal string Calculate()
  {
    return "calculated";
  }
}

public interface IAccountService
{
  string Calculate();
}

public interface IPhoneService
{
  string Calculate();
}

public interface ITaxService
{
  string Calculate();
}

public interface IMarketingService
{
  string Calculate();
}

public interface IReportingService
{
  string Calculate();
}

public class TaxService : ITaxService
{
  public string Calculate() => string.Empty;
}

public class PhoneService : IPhoneService
{
  public string Calculate() => string.Empty;
}