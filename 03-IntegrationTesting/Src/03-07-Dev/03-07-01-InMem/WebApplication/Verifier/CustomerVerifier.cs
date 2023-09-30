using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Maybe;
using WebApplication.Customers;

namespace WebApplication.Verifier;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik
/// w bazie danych. W przypadku wcześniejszego zapisu wyciąga
/// zawartość z bazy danych. Jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat
/// zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;
  private readonly IBikVerificationService _bikVerificationService;
  private readonly IVerificationRepository _repository;
  private readonly IFraudAlertNotifier _fraudAlertNotifier;

  public CustomerVerifier(IBikVerificationService bikVerificationService,
    IReadOnlyCollection<IVerification> verifications,
    IVerificationRepository repository, IFraudAlertNotifier fraudAlertNotifier)
  {
    _bikVerificationService = bikVerificationService;
    _verifications = verifications;
    _repository = repository;
    _fraudAlertNotifier = fraudAlertNotifier;
  }

  /// <summary>
  /// Główna metoda biznesowa. Sprawdza, czy już nie doszło do weryfikacji klienta i jeśli
  /// rezultat zostanie odnaleziony w bazie danych to go zwraca. W innym przypadku zapisuje
  /// wynik weryfikacji w bazie danych. Weryfikacja wówczas zachodzi poprzez odpytanie
  /// BIKu o stan naszego klienta.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    return await _repository.FindByUserId(customer.Guid)
      .Select(p => Task.FromResult(new CustomerVerificationResult(
        p.UserId,
        Enum.Parse<VerificationStatus>(p.Status)))
      ).OrElse(() => CalculateResult(customer));
  }

  private async Task<CustomerVerificationResult> CalculateResult(Customer customer)
  {
    var externalResult = await _bikVerificationService.Verify(customer);
    var result = Result(customer, externalResult);
    await _repository.SaveAsync(VerifiedPerson.CreateInstance(
      customer.Guid,
      customer.Person.NationalIdentificationNumber,
      result.Status));

    if (!result.Passed())
    {
      _fraudAlertNotifier.FraudFound(new CustomerVerification(customer.Person, result));
    }

    return result;
  }

  private CustomerVerificationResult Result(Customer customer, CustomerVerificationResult externalResult)
  {
    if (_verifications.All(verification => verification.Passes(customer.Person)) &&
        externalResult.Passed())
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }
}