using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Customers;

namespace Core.Verifier.Model;

public interface ICustomerVerifier
{
  Task<IReadOnlyList<VerificationResult>> Verify(
    Customer customer, 
    CancellationToken cancellationToken);
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

  /// <summary>
  /// Wykonuje weryfikacje w wielu wątkach.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>zadanie liczące rezultaty weryfikacji</returns>
  public async Task<IReadOnlyList<VerificationResult>> Verify(
    Customer customer, CancellationToken cancellationToken)
  {
    var result = (await Task.WhenAll(
        _verifications.Select(v => v.Passes(customer.Person, cancellationToken))))
      .ToList();
    return result;
  }
}