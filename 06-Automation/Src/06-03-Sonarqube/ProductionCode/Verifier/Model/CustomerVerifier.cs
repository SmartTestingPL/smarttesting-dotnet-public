using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Maybe;
using Microsoft.Extensions.Logging;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Model;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik w bazie danych.
/// Jeśli, przy którejś okaże się, że użytkownik jest oszustem, wówczas
/// odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly BikVerificationService _bikVerificationService;
  private readonly IVerificationRepository _repository;
  private readonly IFraudAlertNotifier _fraudAlertNotifier;
  private readonly ILogger<CustomerVerifier> _logger;
  private readonly Random _random = new();

  public CustomerVerifier(
    BikVerificationService bikVerificationService,
    IVerificationRepository repository,
    IFraudAlertNotifier fraudAlertNotifier,
    ILogger<CustomerVerifier> logger)
  {
    _bikVerificationService = bikVerificationService;
    _repository = repository;
    _fraudAlertNotifier = fraudAlertNotifier;
    _logger = logger;
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <param name="cancellationToken"></param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(Customer customer, CancellationToken cancellationToken)
  {
    return await (await ByUserId(customer.Guid))
      .Select(p => Task.FromResult(new CustomerVerificationResult(
        p.UserId,
        Enum.Parse<VerificationStatus>(p.Status)))
      ).OrElse(() => CalculateResult(customer, cancellationToken));
  }

  private async Task<Maybe<VerifiedPerson>> ByUserId(Guid id)
  {
    try
    {
      await Task.Delay(_random.Next(1000) + 100);
      return _repository.FindByUserId(id);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Exception occurred while trying to fetch results from DB. Will assume fraud");
      return VerifiedPerson.CreateInstance(
        id,
        string.Empty,
        VerificationStatus.VerificationFailed).Just();
    }
  }

  private async Task<CustomerVerificationResult> CalculateResult(Customer customer,
    CancellationToken cancellationToken)
  {
    var externalResult = await _bikVerificationService.Verify(customer, cancellationToken);
    var result = Result(customer, externalResult);
    await _repository.SaveAsync(VerifiedPerson.CreateInstance(
      customer.Guid,
      customer.Person.NationalIdentificationNumber,
      result.Status));

    if (!result.Passed())
    {
      _fraudAlertNotifier.FraudFound(new(customer.Person, result));
    }

    return result;
  }

  private static CustomerVerificationResult Result(
    Customer customer, 
    CustomerVerificationResult externalResult)
  {
    return CustomerVerificationResult.Passed(customer.Guid);
  }
}