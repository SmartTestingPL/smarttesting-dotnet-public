using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Customers;

namespace WebApplication.Verifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas wysyłamy wiadomość do brokera,
/// z informacją o oszuście.
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  private readonly IVerificationRepository _repository;

  private readonly IFraudAlertNotifier _fraudAlertNotifier;

  public CustomerVerifier(
    IReadOnlyCollection<IVerification> verifications,
    IVerificationRepository repository,
    IFraudAlertNotifier fraudAlertNotifier)
  {
    _verifications = verifications;
    _repository = repository;
    _fraudAlertNotifier = fraudAlertNotifier;
  }

  /// <summary>
  /// Główna metoda biznesowa. Weryfikuje czy dana osoba jest oszustem.
  /// W pozytywnym przypadku (jest oszustem) wysyła wiadomość do brokera.
  /// Zapisuje rezultat weryfikacji w bazie danych.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  public async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    if (!IsFraud(customer))
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    var result = CustomerVerificationResult.Failed(customer.Guid);
    // wyślij event jeśli znaleziono oszusta
    _fraudAlertNotifier.FraudFound(
      new CustomerVerification(customer.Person, result));
    await StorePerson(customer, result);
    return result;
  }

  private Task StorePerson(Customer customer, CustomerVerificationResult result)
  {
    return _repository.SaveAsync(VerifiedPerson.CreateInstance(
      customer.Guid,
      customer.Person.NationalIdentificationNumber,
      result.Status));
  }

  private bool IsFraud(Customer customer)
  {
    return !_verifications.Any(
      verification => verification.Passes(customer.Person));
  }
}