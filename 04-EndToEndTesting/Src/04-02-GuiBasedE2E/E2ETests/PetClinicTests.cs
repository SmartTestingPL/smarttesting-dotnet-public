using Bogus;
using E2ETests.Lib;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace E2ETests;

/// <summary>
/// Przykład źle napisanego testu z wykorzystaniem WebDrivera.
/// </summary>
public class PetClinicTests
{
  private IWebDriver _driver = default!;

  [SetUp]
  public void SetUp()
  {
    _driver = new ChromeDriver();
    _driver.Navigate().GoToUrl("http://localhost:8080");
  }

  /// <summary>
  /// Test co prawda coś weryfikuje, ale jest bardzo nieczytelny
  /// i trudny do utrzymania.
  /// </summary>
  [Test]
  public void ShouldAddOwner()
  {
    var person = new Person();

    var findOwners = new WebDriverWait(_driver, 20.Seconds())
      .Until(driver => driver.FindElement(By.LinkText("FIND OWNERS")));
    findOwners.Click();
    // Zastosowanie waitów przy przejściach na kolejne strony
    var addOwnerButton = new WebDriverWait(_driver, 20.Seconds())
      .Until(driver => driver.FindElement(By.LinkText("Add Owner")));
    addOwnerButton.Click();
    var firstNameInput = new WebDriverWait(_driver, 20.Seconds())
      .Until(driver => driver.FindElement(By.Id("firstName")));
    firstNameInput.SendKeys(person.FirstName);
    var lastNameInput = _driver.FindElement(By.Id("lastName"));
    lastNameInput.SendKeys(person.LastName);
    var addressInput = _driver.FindElement(By.Id("address"));
    addressInput.SendKeys(person.Address.Street);
    var cityInput = _driver.FindElement(By.Id("city"));
    cityInput.SendKeys(person.Address.City);
    var telephoneInput = _driver.FindElement(By.Id("telephone"));
    telephoneInput.SendKeys(PhoneNumber.Of(person));
    var addOwnerSubmit = _driver
      .FindElement(By.XPath("/html/body/div/div/form/div[2]/div/button"));
    addOwnerSubmit.Click();
    new WebDriverWait(_driver, 20.Seconds())
      .Until(driver => driver.PageSource.Contains("Owner Information"));

    _driver.PageSource.Should().Contain(person.FullName);
  }

  /// <summary>
  /// Zamknięcie Driver'a (i okna przeglądarki) po teście
  /// </summary>
  [TearDown]
  public void TearDown()
  {
    _driver.Quit();
  }
}