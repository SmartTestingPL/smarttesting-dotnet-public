using System;
using FluentAssertions.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace E2ETests.PageObjects;

/// <summary>
/// Klasa bazowa dla wszystkich klas reprezentujących poszczególne
/// Strony. Zawiera setup webDrivera i weryfikacji czy strona została
/// już załadowana.
/// </summary>
public abstract class PageObject
{
  protected readonly IWebDriver Driver;

  protected PageObject(IWebDriver driver)
  {
    Driver = driver;
    // Metoda z użyciem waita weryfikująca czy strona została załadowana
    PageReady();
  }

  public bool ContainsText(string text)
  {
    return Driver.PageSource.Contains(text);
  }

  /// <summary>
  /// Metoda z użyciem waita weryfikująca
  /// czy strona została załadowana.
  /// </summary>
  private void PageReady()
  {
    var pageReadyCondition = new Func<IWebDriver, bool>(d => ((IJavaScriptExecutor)d)
      .ExecuteScript("return document.readyState;")
      .Equals("complete"));
    var wait = new WebDriverWait(Driver, 20.Seconds());
    wait.Until(pageReadyCondition);
  }
}