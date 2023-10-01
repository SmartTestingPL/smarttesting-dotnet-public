using System;
using WebApplication.Client;

namespace IntegrationTests;

/// <summary>
/// Serwis aplikacyjny, który nie przyjmuje żadnych produkcyjnych
/// weryfikacji. Nadpisujemy jego logikę biznesową w taki sposób,
/// żeby zwrócone zostały dane "na sztywno". Wartość 10 została wzięta
/// w losowy sposób, dla testów.
/// </summary>
/// <returns>serwis aplikacyjny, nie przechodzący przez kolejne warstwy aplikacji</returns>
public class FakeCustomerVerifier : ICustomerVerifier
{
  public CustomerVerificationResult Verify(Person person)
  {
    if (person.GetAge() < 10)
    {
      return CustomerVerificationResult.Failed(Guid.NewGuid());
    }
    return CustomerVerificationResult.Passed(Guid.NewGuid());
  }
}