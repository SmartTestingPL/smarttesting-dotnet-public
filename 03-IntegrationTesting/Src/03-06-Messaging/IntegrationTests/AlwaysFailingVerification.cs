using WebApplication.Customers;
using WebApplication.Verifier;

namespace IntegrationTests;

/// <summary>
/// Weryfikacja, która zawsze jest negatywna - klient chce nas oszukać.
/// </summary>
internal class AlwaysFailingVerification : IVerification
{
  public bool Passes(Person person)
  {
    return false;
  }
}