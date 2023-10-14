using NUnit.Framework;

namespace CoreTests.Lesson2;

/// <summary>
/// Kod do slajdu [Jaki problem chcemy rozwiązać?]
/// </summary>
// ReSharper disable once TestFileNameWarning
public class _02_BrokenDomainTests
{
  /// <summary>
  /// Tu mamy przykład jak w kodzie backendowym możemy tworzyć elementy
  /// UI. Niestety, skoro w tym samym kodzie mamy dostęp do innych
  /// komponentów, np. do repozytoriów nad bazami danych. Nic nie
  /// szkodzi na przeszkodzie, żeby z kodu kliknięcia w przycisk
  /// uruchomić jakieś zapytanie SQL...
  /// </summary>
  [Test]
  public void ShouldPresentBrokenDomain()
  {
    var ui = new Ui(new Button(new Repository()));
    if (ui.UserClicked())
    {
      // z UI zapisz w bazie danych
      var loan = ui.Button.Repository.Save(new Loan());
      ui.Button.ShowInUi(loan.Client.Marketing.HomeAddress);
    }
  }
}

internal class Ui
{
  internal readonly Button Button;

  internal Ui(Button button)
  {
    Button = button;
  }

  internal bool UserClicked()
  {
    return true;
  }

  internal Loan PickedLoan()
  {
    return null;
  }
}

internal class Button
{
  internal readonly Repository Repository;

  internal Button(Repository repository)
  {
    Repository = repository;
  }

  internal void ShowInUi(HomeAddress homeAddress)
  {

  }
}

internal class Repository
{
  internal T Save<T>(T loan)
  {
    return loan;
  }
}

internal class Client
{
  internal readonly Marketing Marketing = new Marketing();
}

internal class Marketing
{
  internal readonly HomeAddress HomeAddress = new HomeAddress();
}

internal class Discounts
{

}

internal class HomeAddress
{

}

internal class Loan
{
  internal readonly Client Client = new Client();
  internal readonly Marketing Marketing = new Marketing();
  internal readonly Discounts Discounts = new Discounts();
}