using OpenQA.Selenium;

namespace E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę startową.
/// </summary>
public class HomePage
{
  private readonly IWebDriver _driver;

  private IWebElement FindOwnersLink => _driver.FindElement(By.LinkText("FIND OWNERS"));

  /// <summary>
  /// Obiekt `IWebDriver` przekazywany w konstruktorach do obiektów
  /// kolejnych stron
  /// </summary>
  public HomePage(IWebDriver driver)
  {
    _driver = driver;
  }

  /// <summary>
  /// Zwraca obiekt kolejnej strony
  /// </summary>
  public FindOwnersPage NavigateToFindOwners()
  {
    FindOwnersLink.Click();
    return new FindOwnersPage(_driver);
  }

}