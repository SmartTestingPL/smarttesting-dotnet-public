using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using ProductionCode.Customers;
using ProductionCode.Db;
using ProductionCode.Lib;
using ProductionCode.Orders;
using TddXt.XNSubstitute;

namespace UnitTests.Orders;

/// <summary>
/// Klasa zawiera przykłady różnych sposobów setupu i tear-downu testów
/// i przykłady zastosowania stubów i mocków.
/// </summary>
public class LoanOrderServiceTest : LoanOrderTestBase
{
  private LoanOrderService _loanOrderService = default!;
  private IMongoDbAccessor _mongoDbAccessor = default!;
  private IPostgresAccessor _postgresAccessor = default!;
  private Customer _student = default!;

  // Metoda setupująca frameworku wywoływana raz przed wywołaniem
  // którejkolwiek metody testowej w klasie.
  [OneTimeSetUp]
  public static void BeforeAllTests()
  {
    Console.WriteLine("Running the tests.");
  }

  // Alternatywne sposoby setupu testów: metoda setupująca frameworku wywoływana przed każdym testem.
  //
  // W odpowiedniku Javowym tych przykładów część pól tej klasy była inicjowana 
  // bezpośrednio za pomocą inicjalizatorów, ale NUnit tworzy jedną instancję klasy testowej
  // na wszystkie testy w przeciwieństwie do JUnita, który tworzy osobną instancję na każdy test.
  // Dlatego umieszczenie inicjalizacji w polach zmieniłoby logikę testu.
  [SetUp]
  public void SetUp()
  {
    _student = AStudent();

    // Tworzenie obiektów stub/ mock

    // Mock, który będzie wykorzystywany później do weryfikacji interakcji
    _postgresAccessor = Substitute.For<IPostgresAccessor>();

    // Ten obiekt tak naprawdę jest wyłącznie stubem (nie używamy go do weryfikacji interakcji).
    // NSubstitute sam w sobie nie rozróżnia między stubami i mockami - dla niego wszystko
    // to substytuty.
    _mongoDbAccessor = Substitute.For<IMongoDbAccessor>();

    _loanOrderService = new LoanOrderService(_postgresAccessor, _mongoDbAccessor);

    // Stubowanie metody GetPromotionDiscount(...)
    _mongoDbAccessor.GetPromotionDiscount("Student Promo").Returns(10);
  }

  // Metoda Tear down wywoływana po zakończeniu wszystkich testów w klasie.
  [OneTimeTearDown]
  public static void AfterAllTests()
  {
    Console.WriteLine("Finished running the tests.");
  }

  // Testowanie wyniku operacji
  [Test]
  public void ShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.OrderDate.Should().Be(Clocks.ZonedUtc.GetCurrentDate());
    loanOrder.Promotions.Should().ContainSingle(promotion => promotion.Name == "Student Promo");
    loanOrder.Promotions.Should().HaveCount(1);
    loanOrder.Promotions[0].Discount.Should().Be(10);
  }

  [Test]
  public void ShouldUpdatePromotionStatistics()
  {
    _loanOrderService.StudentLoanOrder(_student);

    // Weryfikacja interakcji z użyciem obiektu, który jest też stosowany jako stub
    _postgresAccessor.Received(1).UpdatePromotionStatistics("Student Promo");

    // Weryfikacja tego, że dana interakcja nie wystąpiła
    _postgresAccessor.DidNotReceive()
      .UpdatePromotionDiscount("Student promo", Arg.Any<decimal>());

    // Alternatywna asercja wobec tych dwóch powyżej: można zweryfikować, czy nie nastąpiła
    // żadna inna interakcja na danym mocku prócz tych, których się spodziewamy (dla przykładu
    // pokazujemy oba sposoby, ale normalnie byłoby to stosowane zamiast tej asercji powyżej).
    // NSubstitute nie ma wbudowanej takiej funkcji, ale jakiś czas temu zrobiłem ją
    // na własne potrzeby. Możesz podglądnąć kod na githubie i dostosować.
    XReceived.Only(() => _postgresAccessor.UpdatePromotionStatistics("Student Promo"));
  }

  // Przykład AssertObject Pattern
  [Test]
  public void AssertObjectShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    var orderShould = new LoanOrderAssert(loanOrder);
    orderShould.BeRegisteredToday();
    orderShould.HavePromotion("Student Promo");
    orderShould.HaveOnlyOnePromotion();
    orderShould.HaveOnFirstPromotionDiscountValueOf(10);
  }

  // Przykład AssertObject Pattern z łańcuchowaniem asercji.
  // Można by też zrobić z tego rozszerzenie do FluentAssertions,
  // ale nie ma takiego przymusu.
  [Test]
  public void ChainedAssertObjectShouldCreateStudentLoanOrder()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.Should().BeRegisteredToday()
      .HavePromotion("Student Promo")
      .HaveOnlyOnePromotion()
      .HaveOnFirstPromotionDiscountValueOf(10);
  }

  // Przykład AssertObject Pattern z zastosowaniem metody opakowującej łańcuch asercji
  [Test]
  public void ChainedAssertObjectShouldCreateStudentLoanOrderSimpleAssertion()
  {
    var student = AStudent();

    var loanOrder = _loanOrderService.StudentLoanOrder(student);

    loanOrder.Should().BeCorrect();
  }
}