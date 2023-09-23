using System.Collections.Generic;
using System.Linq;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zwraca zagregowany wynik.
///
/// Klasa używa obiektu-wrappera otaczającego metodę statyczną realizującą
/// operacje bazodanowe.
/// 
/// Nie polecamy robienia czegoś takiego w metodzie statycznej, ale tu pokazujemy
/// jak to obejść i przetestować, jeżeli z jakiegoś powodu nie da się tego zmienić
/// (np. metoda statyczna jest dostarczana przez kogoś innego).
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(IReadOnlyCollection<IVerification> verifications)
  {
    _verifications = verifications;
  }

  public CustomerVerificationResult Verify(Customer customer)
  {
    if (_verifications.All(verification => verification.Passes(customer.Person)))
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}