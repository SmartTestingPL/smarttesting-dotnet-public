using OpenQA.Selenium;

namespace E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę właściciela.
/// </summary>
public class OwnerViewPage : PageObject
{
  /// <summary>
  /// Obiekt `IWebDriver` przekazywany w konstruktorach do obiektów
  /// kolejnych stron
  /// </summary>
  public OwnerViewPage(IWebDriver driver) : base(driver)
  {
  }

  public string Text => Driver.PageSource;
}