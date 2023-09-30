using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Client;

public interface ICustomerVerifier
{
  CustomerVerificationResult Verify(Person person);
}

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier : ICustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(IReadOnlyCollection<IVerification> verifications)
  {
    _verifications = verifications;
  }

  /// <summary>
  /// Główna metoda biznesowa. Weryfikuje czy dana osoba jest oszustem.
  /// </summary>
  /// <param name="person">osoba do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public CustomerVerificationResult Verify(Person person)
  {
    if (_verifications.All(verification => verification.Passes(person)))
    {
      return CustomerVerificationResult.Passed(person.Guid);
    }

    return CustomerVerificationResult.Failed(person.Guid);
  }
}