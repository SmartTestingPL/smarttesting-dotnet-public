using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
/// Jeśli przy którejś okaże się, że użytkownik jest oszustem, wówczas
/// odpowiedni rezultat zostanie zwrócony.
/// </summary>
public class _01_CustomerVerifier : ICustomerVerifier
{
  private readonly IReadOnlyCollection<IVerification> _verifications;
  private readonly IFraudAlertNotifier _fraudAlertNotifier;
  private readonly IScheduler _scheduler;

  public _01_CustomerVerifier(
    IReadOnlyCollection<IVerification> verifications,
    IFraudAlertNotifier fraudAlertNotifier,
    IScheduler scheduler)
  {
    _verifications = verifications;
    _fraudAlertNotifier = fraudAlertNotifier;
    _scheduler = scheduler;
  }

  /// <summary>
  /// Wykonuje weryfikacje w wielu wątkach.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultaty w kolejności ukończenia</returns>
  public async Task<IReadOnlyList<VerificationResult>> Verify(Customer customer, CancellationToken cancellationToken)
  {
    var result = new ConcurrentBag<VerificationResult>();
    var tasks = _verifications.Select(v => v.PassesAsync(customer.Person, FiveSecondTimeout())
      .ContinueWith(async task =>
      {
        result.Add(await task);
      }, cancellationToken));

    await Task.WhenAll(tasks);
    return result.ToList();
  }

  /// <summary>
  /// Wykonuje weryfikacje w wielu wątkach.
  /// Nie pozwala rzucić wyjątku w przypadku błędu w procesowaniu.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultaty w kolejności ukończenia</returns>
  public async Task<IReadOnlyList<VerificationResult>> VerifyNoException(
    Customer customer,
    CancellationToken cancellationToken)
  {
    var result = new ConcurrentBag<VerificationResult>();
    var tasks = _verifications.Select(async v =>
    {
      try
      {
        await v.PassesAsync(customer.Person, FiveSecondTimeout())
          .ContinueWith(
            async task =>
            {
              result.Add(await task);
            }, cancellationToken).Unwrap();
      }
      catch
      {
        result.Add(new VerificationResult(v.Name, false));
      }
    });

    await Task.WhenAll(tasks);
    return result.ToList();
  }

  /// <summary>
  /// Wykonuje weryfikacje w sposób reaktywny
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultaty w kolejności ukończenia</returns>
  public IObservable<VerificationResult> VerifyRx(Customer customer)
  {
    return _verifications.ToObservable()
      .Select(v => v.Passes(customer.Person));
  }

  /// <summary>
  /// Wykonuje weryfikacje w sposób reaktywny
  /// i wielowątkowy
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultaty w kolejności ukończenia</returns>

  public IObservable<VerificationResult> VerifyParallelRx(Customer customer)
  {
    var selectMany = _verifications
      .Select(v => Observable.Start(() => v.Passes(customer.Person), _scheduler))
      .Merge(3);
    return selectMany;
  }

  private static CancellationToken FiveSecondTimeout()
  {
    var timeoutTokenSource = new CancellationTokenSource();
    timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
    return timeoutTokenSource.Token;
  }

  /// <summary>
  /// Rozpoczyna weryfikacje w wielu wątkach.
  /// Nazwa metody ma końcówkę "Async" - w tym przypadku
  /// wynika to za podążaniem za nazewnictwem z przykładu Javowego.
  /// Metoda nie zwraca <see cref="Task"/>.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  /// <returns>rezultaty w kolejności ukończenia</returns>
  public void VerifyAsync(Customer customer)
  {
    foreach (var verification in _verifications)
    {
      _ = Task.Run(() => verification.Passes(customer.Person));
    }
  }

  /// <summary>
  /// Wysyła notyfikację o znalezionym oszuście.
  /// </summary>
  /// <param name="customer">klient do zweryfikowania</param>
  public void FoundFraud(Customer customer)
  {
    _ = _fraudAlertNotifier
      .FraudFound(
        new CustomerVerification(
          customer.Person,
          CustomerVerificationResult.Failed(customer.Guid)));
  }
}