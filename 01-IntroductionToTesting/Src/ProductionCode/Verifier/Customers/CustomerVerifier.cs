using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zwraca zagregowany wynik.
///
/// Klasa używa obiektu-wrappera (_serviceWrapper) otaczającego metodę
/// statyczną realizującą operacje bazodanowe.
/// Nie polecamy robienia czegoś takiego w metodzie statycznej,
/// ale tu pokazujemy jak to obejść i przetestować
/// jeżeli z jakiegoś powodu nie da się tego zmienić
/// (np. metoda statyczna jest dostarczana przez kogoś innego).
/// </summary>
public class CustomerVerifier
{
  private readonly BikVerificationService _bikVerificationService;
  private readonly VeryBadVerificationServiceWrapper _serviceWrapper;
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(BikVerificationService bikVerificationService,
    IReadOnlyCollection<IVerification> verifications, VeryBadVerificationServiceWrapper serviceWrapper)
  {
    _bikVerificationService = bikVerificationService;
    _verifications = verifications;
    _serviceWrapper = serviceWrapper;
  }

  public async Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
  {
    var externalResult = await _bikVerificationService.Verify(customer, cancellationToken);

    if (_verifications.All(verification => verification.Passes(customer.Person))
        && externalResult.Passed()
        && await _serviceWrapper.Verify(cancellationToken)) //użycie wrappera
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}