using NSelene;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static NSelene.Selene;

namespace E2ETests;

/// <summary>
/// Przykład testu z wykorzystaniem NSelene.
/// 
/// NSelene jest ubogim portem Selenide, użytego
/// w Javowej wersji.
/// </summary>
public class PetClinicNSeleneTests
{
  [OneTimeSetUp]
  public void InitDriver()
  {
    SetWebDriver(new ChromeDriver());
  }

  [OneTimeTearDown]
  public void QuitDriver()
  {
    GetWebDriver().Quit();
  }

  [SetUp]
  public void SetUp()
  {
    Open("http://localhost:8080");
  }

  [Test]
  public void ShouldDisplayErrorMessage()
  {
    S(By.LinkText("ERROR")).Click();

    S(ByText("Something happened...")).Should(Be.Visible);
  }

  /// <summary>
  /// Wersja Javowa Selenide ma wbudowane wyszukiwanie
  /// po tekście. Nie znalazłem tego w NSelene, więc
  /// przeportowałem.
  /// </summary>
  private static By ByText(string text)
  {
    return By.XPath(".//*/text()[normalize-space(translate(string(.), '\t\n\r ', '    '))" +
                    $" = '{text}']/parent::*");
  }
}