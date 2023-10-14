using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using FluentAssertions;
using static MainSettings;
using static RunContractTests.RelevantPaths;
using static SimpleExec.Command;

namespace RunContractTests;

internal static class ProducerTests
{
  // Zestawienie pełnego URL Twojej aplikacji
  private const string ApplicationBaseUrl = "http://host.docker.internal:5000";

  /// <summary>
  /// Przeprowadza testy kontraktowe producenta
  /// </summary>
  /// <param name="cancellationToken"></param>
  public static async Task RunTests(CancellationToken cancellationToken)
  {
    Console.WriteLine("Spring Cloud Contract Version " +
                      $"[{SpringCloudContractsDockerVersion}]");
    Console.WriteLine($"Application URL [{ApplicationBaseUrl}]");
    Console.WriteLine($"Project Version [{ProjectVersion}]");

    //Ustawiamy ścieżkę roboczą na folder producenta
    GetProducerAppPath().SetAsCurrentDirectory();

    //Kasujemy wynik poprzedniego odpalenia (jeśli tak był)
    DeleteResultsOfPreviousRuns();

    IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
      .Should().NotContain(l => l.Port == 5000, "the application needs it to run");

    // Odpalamy aplikację w tle. Tym razem używamy standardowego
    // mechanizmu .netowego do uruchamiania procesów, bo SimpleExec nie
    // umożliwia (póki co) zamykania uruchomionych procesów.
    using var appProcess = Process.Start(new ProcessStartInfo("dotnet", "run")
    {
      UseShellExecute = false,
    });

    try
    {
      //Uruchamiamy testy kontraktowe
      await RunContractTests(cancellationToken);
    }
    finally
    {
      appProcess?.Kill(true);
    }
  }

  private static void DeleteResultsOfPreviousRuns()
  {
    DeleteDirectory(BuildDir);
  }

  private static void DeleteDirectory(AbsoluteDirectoryPath directory)
  {
    try
    {
      if (directory.Exists())
      {
        directory.Delete(true);
      }
    }
    catch (IOException ex) when (ex.Message.Contains("Directory not empty"))
    {
      Console.WriteLine("==================================");
      Console.WriteLine("ERROR: current user doesn't have the privileges to remove files from previous run.");
      Console.WriteLine($"SOLUTION: manually run: sudo rm -R \"{directory}\" and rerun this script");
      Console.WriteLine("==================================");
      throw;
    }
  }

  /// <summary>
  /// Funkcja uruchamiająca testy kontraktowe
  /// Zakłada:
  /// * [PUBLISH_ARTIFACTS=false] - nie publikujemy zaślepek do zewnętrznego
  ///   miejsca (np. Artifactory)
  /// * [PUBLISH_ARTIFACTS_OFFLINE=true] - publikujemy zaślepki do lokalnego
  ///   repozytorium Mavena (na potrzeby demonstracji). Moglibyśmy trzymać
  ///   je też w Artifactory lub w Gicie
  /// * kontrakty zostały zdefiniowane w podkatalogu [contracts]
  /// * efekty uruchomienia testów kontraktowych będą obecne w podkatalogu
  ///   [build/spring-cloud-contract/output]
  /// </summary>
  /// <param name="cancellationToken"></param>
  private static async Task RunContractTests(CancellationToken cancellationToken)
  {
    await RunAsync("docker", 
      "run --rm " +
      $" -e \"APPLICATION_BASE_URL={ApplicationBaseUrl}\"" + 
      " -e \"PUBLISH_ARTIFACTS=false\"" +
      $" -e \"PROJECT_NAME={ProjectName}\"" +
      $" -e \"PROJECT_GROUP={ProjectGroup}\"" +
      $" -e \"PROJECT_VERSION={ProjectVersion}\"" +
      $" -v \"{GetProducerPath()}/Contracts/:/contracts:ro\"" +
      $" -v \"{SpringCloudContractOutputDir}:/spring-cloud-contract-output/\"" +
      $" springcloud/spring-cloud-contract:\"{SpringCloudContractsDockerVersion}\"", 
      cancellationToken: cancellationToken);
  }
}