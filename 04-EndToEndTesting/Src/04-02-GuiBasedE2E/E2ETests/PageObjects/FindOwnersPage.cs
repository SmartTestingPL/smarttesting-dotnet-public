using OpenQA.Selenium;

namespace E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę wyszukiwania właścicieli.
/// </summary>
public class FindOwnersPage : PageObject
{
  private IWebElement AddOwnerSubmit =>
    Driver.FindElement(By.LinkText("Add Owner"));

  /// <summary>
  /// Obiekt `IWebDriver` przekazywany w konstruktorach do obiektów
  /// kolejnych stron
  /// </summary>
  public FindOwnersPage(IWebDriver driver)
    : base(driver)
  {
  }

  /// <summary>
  /// Zwraca obiekt kolejnej strony
  /// </summary>
  public AddOwnerPage NavigateToAddOwner()
  {
    AddOwnerSubmit.Click();
    return new AddOwnerPage(Driver);
  }

}