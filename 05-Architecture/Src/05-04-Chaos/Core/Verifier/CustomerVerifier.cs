using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;
using Core.Maybe;
using Microsoft.Extensions.Logging;

namespace Core.Verifier;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik w bazie danych.
/// Jeśli, przy którejś okaże się, że użytkownik jest oszustem, wówczas
/// odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;
  private readonly IBikVerificationService _bikVerificationService;
  private readonly IVerificationRepository _repository;
  private readonly ILogger<CustomerVerifier> _logger;
  private readonly IFraudAlertNotifier _fraudAlertNotifier;


  public CustomerVerifier(
    IBikVerificationService bikVerificationService,
    IReadOnlyCollection<IVerification> verifications,
    IVerificationRepository repository,
    IFraudAlertNotifier fraudAlertNotifier,
    ILogger<CustomerVerifier> logger)
  {
    _bikVerificationService = bikVerificationService;
    _verifications = verifications;
    _repository = repository;
    _fraudAlertNotifier = fraudAlertNotifier;
    _logger = logger;
  }

  /// <summary>
  /// Weryfikuje czy dana osoba jest oszustem.
  ///
  /// Najpierw łączymy się do bazy danych, żeby znaleźć daną osobę po id.
  /// Jeśli dana osoba była już zapisana w bazie, to zwracamy zapisany rezultat,
  /// w innym razie wykonujemy zapytanie do usługi zewnętrznej i również dokonujemy zapisu.
  ///
  /// Mamy tu problem w postaci braku obsługi jakichkolwiek wyjątków. Co jeśli baza danych
  /// rzuci wyjątkiem? Jak powinniśmy to obsłużyć biznesowo?
  ///
  /// Po pierwszym uruchomieniu eksperymentu z dziedziny inżynierii chaosu, który wywali test
  /// dot. bazy danych, zakomentuj linijkę
  /// <code>return await _repository.FindByUserId(customer.Guid)</code>
  /// i odkomentuj tę:
  /// <code>return await ByUserId(customer.Guid)</code>
  /// wówczas jeden z dwóch testów przejdzie, ponieważ obsługujemy poprawnie błędy bazodanowe.
  /// </summary>
  /// <param name="customer">osoba do zweryfikowania</param>
  /// <param name="cancellationToken"></param>
  /// <returns>rezultat weryfikacji</returns>
  public async Task<CustomerVerificationResult> Verify(
    Customer customer, CancellationToken cancellationToken)
  {
      return await _repository.FindByUserId(customer.Guid)
      //return await ByUserId(customer.Guid)
      .Select(c => Task.FromResult(ToResult(c)))
      .OrElse(() => CalculateResult(customer, cancellationToken));
  }

  private static CustomerVerificationResult ToResult(VerifiedPerson p)
  {
    return new CustomerVerificationResult(
      p.UserId,
      Enum.Parse<VerificationStatus>(p.Status));
  }

  /// <summary>
  /// Poprawiona wersja odpytania o wyniki z bazy danych. Jeśli wyjątek został rzucony,
  /// zalogujemy go, ale z punktu widzenia biznesowego możemy spokojnie założyć, że
  /// mamy do czynienia z potencjalnym oszustem.
  /// </summary>
  /// <param name="id">id osoby, której szukaliśmy</param>
  /// <returns>osoba z bazy danych lub potencjalny oszust</returns>
  private Maybe<VerifiedPerson> ByUserId(Guid id)
  {
    try
    {
      return _repository.FindByUserId(id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception occurred while trying to fetch results from DB. Will assume fraud");
      return Verifier.VerifiedPerson.CreateInstance(
        id, string.Empty, VerificationStatus.VerificationFailed).Just();
    }
  }

  private async Task<CustomerVerificationResult> CalculateResult(
    Customer customer,
    CancellationToken cancellationToken)
  {
    var externalResult = await _bikVerificationService.Verify(customer, cancellationToken);
    var result = ToResult(customer, externalResult);
    await _repository.SaveAsync(VerifiedPerson(customer, result), cancellationToken);
    if (result.Passed())
    {
      _fraudAlertNotifier.FraudFound(new CustomerVerification(customer.Person, result));
    }
    return result;
  }

  private static VerifiedPerson VerifiedPerson(Customer customer, CustomerVerificationResult result)
  {
    return Verifier.VerifiedPerson.CreateInstance(customer.Guid,
      customer.Person.NationalIdentificationNumber,
      result.Status);
  }

  private CustomerVerificationResult ToResult(
    Customer customer,
    CustomerVerificationResult externalResult)
  {
    if (_verifications
        .All(verification => verification.Passes(customer.Person)
                             && externalResult.Passed()))
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }

}