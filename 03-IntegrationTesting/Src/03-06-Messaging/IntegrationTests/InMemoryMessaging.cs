using System;
using System.Collections.Concurrent;
using FluentAssertions.Extensions;
using Core.Maybe;
using WebApplication.Verifier.Customers;

namespace IntegrationTests;

/// <summary>
/// Wysyłacz wiadomości, który wrzuca wiadomości na kolejkę w pamięci.
/// </summary>
internal class InMemoryMessaging : IFraudAlertNotifier, IDisposable
{
  private readonly BlockingCollection<CustomerVerification> _broker
    = new BlockingCollection<CustomerVerification>(20);

  public void FraudFound(CustomerVerification customerVerification)
  {
    _broker.Add(customerVerification);
  }

  /// <summary>
  /// Pozwala na wyjęcie weryfikacji z kolejki.
  /// </summary>
  /// <returns>ostatni element wrzucony na kolejkę lub null jeśli brak</returns>
  public Maybe<CustomerVerification> Poll()
  {
    _broker.TryTake(out var item, 100.Milliseconds());
    return item!.ToMaybe();
  }

  public void Dispose()
  {
    _broker.Dispose();
  }
}