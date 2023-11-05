using System.Collections.Generic;
using System.Linq;
using Core.Customers;

namespace Core.Verifier.Model;

public interface ICustomerVerifier
{
  IEnumerable<VerificationResult> Verify(Customer customer);
}

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i zapisuje jej wynik w bazie danych.
/// Jeśli, przy którejś okaże się, że użytkownik jest oszustem, wówczas
/// odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class CustomerVerifier : ICustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;

  public CustomerVerifier(IReadOnlyCollection<IVerification> verifications)
  {
    _verifications = verifications;
  }

  public IEnumerable<VerificationResult> Verify(Customer customer)
  {
    return _verifications.Select(v => v.Passes(customer.Person)).ToList();
  }
}