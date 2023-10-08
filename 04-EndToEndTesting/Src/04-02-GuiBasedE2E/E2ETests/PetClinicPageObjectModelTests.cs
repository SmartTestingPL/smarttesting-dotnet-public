using Bogus;
using E2ETests.Lib;
using E2ETests.PageObjects;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace E2ETests;

/// <summary>
/// Przykład zastosowania wzorca PageObjectModel.
/// </summary>
public class PetClinicPageObjectModelTests
{
  // Sterownik przeglądarki ściągany jest poprzez pakiet NuGet.
  // Wersja nugeta z sterownikiem musi odpowiadać wersji Chrome'a
  // która jest zainstalowana na komputerze!!!!
  private IWebDriver _driver = default!;
  private HomePage _homePage = default!;

  [SetUp]
  public void SetUp()
  {
    _driver = new ChromeDriver();
    _driver.Navigate().GoToUrl("http://localhost:8080");
    _homePage = new HomePage(_driver);
  }

  /// <summary>
  /// Test z wykorzystaniem Selenium PageObjectModel
  /// </summary>
  [Test]
  public void ShouldAddOwner()
  {
    var person = new Person();
    var findOwnersPage = _homePage.NavigateToFindOwners();
    var addOwnerPage = findOwnersPage.NavigateToAddOwner();
    FillOwnerData(addOwnerPage, person);

    var ownerViewPage = addOwnerPage.AddOwner();
    ownerViewPage.ContainsText(person.FullName).Should().BeTrue(
      $"{ownerViewPage.Text} should contain {person.FullName}");
  }

  /// <summary>
  /// Boiler-plate wyniesiony do metody pomocniczej
  /// </summary>
  private void FillOwnerData(AddOwnerPage addOwnerPage, Person person)
  {
    addOwnerPage.FillFirstName(person.FirstName);
    addOwnerPage.FillLastName(person.LastName);
    addOwnerPage.FillAddress(person.Address.Street);
    addOwnerPage.FillCity(person.Address.City);

    addOwnerPage.FillTelephoneNumber(PhoneNumber.Of(person));
  }

  /// <summary>
  /// Zamknięcie Driver'a (i okna przeglądarki) po teście.
  /// </summary>
  [TearDown]
  public void TearDown()
  {
    _driver.Quit();
  }
}