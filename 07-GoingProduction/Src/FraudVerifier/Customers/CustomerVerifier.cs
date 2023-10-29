using System;
using System.Collections.Generic;
using System.Linq;
using FraudVerifier.Customers.Verification;
using Prometheus;
using Unleash;

namespace FraudVerifier.Customers;

/// <summary>
/// Weryfikacja czy klient jest oszustem czy nie. Przechodzi po
/// różnych implementacjach weryfikacji i jeśli, przy którejś okaże się,
/// że użytkownik jest oszustem, wówczas odpowiedni rezultat zostanie zwrócony.
///
/// Zestaw uruchamianych weryfikacji różni się w zależności od ustawień przełącznika
/// funkcji w Unleashu.
/// </summary>
public class CustomerVerifier
{
  /// <summary>
  /// Obiekt do raportowania czasu trwania weryfikacji klienta.
  /// Zaraportowane wartości będą pobierane przez Prometheusa.
  /// </summary>
  private static readonly Histogram VerifyDuration = Metrics.CreateHistogram(
    "verifyCustomerTimer", 
    "Histogram of customer verification durations.");

  private readonly IReadOnlyCollection<IVerification> _verifications;
    
  /// <summary>
  /// Klient przełączników funkcji:
  /// </summary>
  private readonly IUnleash _unleash;

  public CustomerVerifier(
    IReadOnlyCollection<IVerification> verifications, 
    IUnleash unleash)
  {
    _verifications = verifications;
    _unleash = unleash;
  }

  public CustomerVerificationResult Verify(Customer customer)
  {
    using (VerifyDuration.NewTimer())
    {
      var updatedVerifications = _verifications;

      // Jeżeli przełącznik NewVerification jest aktywny, weryfikacja
      // typu NewTypeOfVerification zostanie uruchomiona

      if (_unleash.IsEnabled(Features.NewVerification))
      {
        updatedVerifications = updatedVerifications.Append(new NewTypeOfVerification()).ToList();
      }

      if (updatedVerifications.All(verification => verification.Passes(customer.Person)))
      {
        return CustomerVerificationResult.Passed(customer.Guid);
      }

      return CustomerVerificationResult.Failed(customer.Guid);
    }
  }
}