using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FraudDetection.Customers;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace FraudDetection.Verifier;

public interface ICustomerVerifier
{
  Task<CustomerVerificationResult> Verify(Customer customer);
}

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik
/// w bazie danych. W przypadku wcześniejszego zapisu wyciąga
/// zawartość z bazy danych. Jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat
/// zostanie zwrócony.
/// </summary>
public class CustomerVerifier : ICustomerVerifier
{
  private readonly IReadOnlySet<IVerification> _verifications;
  private readonly IBikVerificationService _bikVerificationService;
  private readonly IVerificationRepository _repository;
  private readonly IFraudAlertNotifier _fraudAlertNotifier;
  private readonly ILogger<CustomerVerifier> _logger;
  private static readonly Histogram CustomerVerificationHistogram = Metrics.CreateHistogram(
    "customer_verification", 
    "Histogram of customer verification durations.");

  public CustomerVerifier(
    IBikVerificationService bikVerificationService,
    IObjectProvider<IReadOnlySet<IVerification>> verifications,
    IVerificationRepository repository, 
    IFraudAlertNotifier fraudAlertNotifier,
    ILogger<CustomerVerifier> logger)
  {
    _bikVerificationService = bikVerificationService;
    _verifications = verifications.GetIfAvailable(() => new HashSet<IVerification>());
    _repository = repository;
    _fraudAlertNotifier = fraudAlertNotifier;
    _logger = logger;
  }

  /// <summary>
  /// Główna metoda biznesowa. Sprawdza, czy już nie doszło do weryfikacji klienta i jeśli
  /// rezultat zostanie odnaleziony w bazie danych to go zwraca. W innym przypadku zapisuje
  /// wynik weryfikacji w bazie danych. Weryfikacja wówczas zachodzi poprzez odpytanie
  /// BIKu o stan naszego klienta.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultat weryfikacji</returns>
  public virtual async Task<CustomerVerificationResult> Verify(Customer customer)
  {
    return await _repository.FindByUserId(customer.Guid)
      .Select(p => Task.FromResult(new CustomerVerificationResult(
        p.UserId,
        Enum.Parse<VerificationStatus>(p.Status)))
      ).OrElse(() => CalculateResult(customer));
  }

  private async Task<CustomerVerificationResult> CalculateResult(Customer customer)
  {
    using (CustomerVerificationHistogram.NewTimer())
    {
      _logger.LogInformation(
        $"Customer with id [{customer.Guid}] not found in the database. Will calculate the new result");
      var externalResult = await _bikVerificationService.Verify(customer);
      _logger.LogInformation($"The result from BIK was [{externalResult}]");
      var result = Result(customer, externalResult);
      _logger.LogInformation($"The result from other checks was [{result}]");
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