using OpenQA.Selenium;

namespace E2ETests.PageObjects;

/// <summary>
/// Reprezentuje stronę dodawania właściciela.
/// </summary>
public class AddOwnerPage : PageObject
{
  private IWebElement FirstNameInput => Driver.FindElement(By.Id("firstName"));
  private IWebElement LastNameInput => Driver.FindElement(By.Id("lastName"));
  private IWebElement AddressInput => Driver.FindElement(By.Id("address"));
  private IWebElement CityInput => Driver.FindElement(By.Id("city"));
  private IWebElement TelephoneNumberInput => Driver.FindElement(By.Id("telephone"));
  private IWebElement AddOwnerSubmit => Driver.FindElement(By.XPath("/html/body/div/div/form/div[2]/div/button"));

  /// <summary>
  /// Obiekt `IWebDriver` przekazywany w konstruktorach do obiektów
  /// kolejnych stron
  /// </summary>
  public AddOwnerPage(IWebDriver driver) : base(driver)
  {
  }

  //
  // Operacje, które można wykonać na stronie:
  //

  public void FillFirstName(string firstName)
  {
    FirstNameInput.SendKeys(firstName);
  }

  public void FillLastName(string lastName)
  {
    LastNameInput.SendKeys(lastName);
  }

  public void FillAddress(string address)
  {
    AddressInput.SendKeys(address);
  }

  public void FillCity(string city)
  {
    CityInput.SendKeys(city);
  }

  public void FillTelephoneNumber(string telephoneNumber)
  {
    TelephoneNumberInput.SendKeys(telephoneNumber);
  }

  /// <summary>
  /// Zwraca obiekt kolejnej strony
  /// </summary>
  public OwnerViewPage AddOwner()
  {
    AddOwnerSubmit.Click();
    return new OwnerViewPage(Driver);
  }

}