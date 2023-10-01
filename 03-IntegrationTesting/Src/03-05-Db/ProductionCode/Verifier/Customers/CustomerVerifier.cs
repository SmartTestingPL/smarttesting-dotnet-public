using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Maybe;
using ProductionCode.Customers;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik w bazie danych.
/// Jeśli, przy którejś okaże się, że użytkownik jest oszustem, wówczas
/// odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier
{
  private readonly ISet<IVerification> _verifications;
  private readonly BikVerificationService _bikVerificationService;
  private readonly IVerificationRepository _repository;

  public CustomerVerifier(
    BikVerificationService bikVerificationService,
    ISet<IVerification> verifications,
    IVerificationRepository repository)
  {
    _bikVerificationService = bikVerificationService;
    _verifications = verifications;
    _repository = repository;
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
      .Select(c => Task.FromResult(ToResult(c)))
      .OrElse(() => VerifyCustomer(customer));
  }

  private static CustomerVerificationResult ToResult(VerifiedPerson p)
  {
    return new CustomerVerificationResult(
      p.UserId,
      Enum.Parse<VerificationStatus>(p.Status));
  }

  private async Task<CustomerVerificationResult> VerifyCustomer(Customer customer)
  {
    var externalResult = await _bikVerificationService.Verify(customer);
    var result = ToResult(customer, externalResult);
    await _repository.SaveAsync(VerifiedPerson(customer, result));
    return result;
  }

  private static VerifiedPerson VerifiedPerson(Customer customer, CustomerVerificationResult result)
  {
    return Customers.VerifiedPerson.CreateInstance(customer.Guid,
      customer.Person.NationalIdentificationNumber,
      result.Status);
  }

  private CustomerVerificationResult ToResult(
    Customer customer, 
    CustomerVerificationResult externalResult)
  {
    if (_verifications
          .All(verification => verification.Passes(customer.Person)) 
        && externalResult.Passed())
    {
      return CustomerVerificationResult.Passed(customer.Guid);
    }

    return CustomerVerificationResult.Failed(customer.Guid);
  }

}