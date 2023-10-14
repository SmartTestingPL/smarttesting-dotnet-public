// Wersja dockera z Spring Cloud Contracts

using System;
using System.Threading;
using RunContractTests;

var cts = new CancellationTokenSource();
RegisterCancel(cts);

await ProducerTests.RunTests(cts.Token);
await ConsumerTests.RunTests(cts.Token);

void RegisterCancel(CancellationTokenSource cancellationTokenSource)
{
  Console.CancelKeyPress += (s, e) =>
  {
    Console.WriteLine("Canceling...");
    cancellationTokenSource.Cancel();
    e.Cancel = true;
  };
}

public static class MainSettings
{
  public const string SpringCloudContractsDockerVersion = "4.0.4";

  // Wersja projektu
  public const string ProjectVersion = "0.0.1-SNAPSHOT";

  // Nazwa grupy, w której Twój projekt się znajduje
  public const string ProjectGroup = "pl.smarttesting";

  // Nazwa Twojego projektu
  public const string ProjectName = "loan-issuance";
}
