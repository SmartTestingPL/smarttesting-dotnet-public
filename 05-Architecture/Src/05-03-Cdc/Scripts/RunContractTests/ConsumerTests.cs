using System;
using System.Threading;
using System.Threading.Tasks;
using AtmaFileSystem.IO;
using FluentAssertions;
using FluentAssertions.Extensions;
using static MainSettings;
using static SimpleExec.Command;

namespace RunContractTests;

internal static class ConsumerTests
{
  // Namiary na stuba: 'groupId:artifactId:version:classifier:port' 
  private static readonly string StubRunnerIds 
    = $"{ProjectGroup}:{ProjectName}:{ProjectVersion}:stubs:9876";
    
  // Obraz stub runnera
  private static readonly string ImageName 
    = "springcloud/spring-cloud-contract-stub-runner:" +
      $"\"{SpringCloudContractsDockerVersion}\"";
    
  // Port stub runnera
  private const string StubRunnerPort = "8083";

  private const string StubRunnerRepositoryRoot = "stubs://file:///scc_output";

  // Nazwa pod którą zostanie wystartowany kontener stub runnera
  private const string StubRunnerContainerName = "StubRunner";

  public static async Task RunTests(CancellationToken cancellationToken)
  {
    var consumerPath = RelevantPaths.GetConsumerPath();
    consumerPath.SetAsCurrentDirectory();
      
    // Setup
    await StopIfAlreadyRunning(StubRunnerContainerName, cancellationToken);
    await InstallAndConfigureNode(cancellationToken);

    // Given
    StartStubRunnerAsDockerContainer(cancellationToken);
    
    //Za pierwszym razem gdy obraz dockerowy jest pobierany, to może nie wystarczyć.
    //Warto wtedy zwiększyć ten czas albo puścić skrypt jeszcze raz.
    Console.WriteLine("Wait for 60 seconds for the container to start");
    await Task.Delay(60.Seconds(), cancellationToken);
      
    try
    {
      var nodeFolder = consumerPath.AddDirectoryName("node");
      var nodeExe = nodeFolder.AddFileName("node");
      {
        // When - fraud
        var fraudResult = (await ReadAsync(nodeExe.ToString(), "../app", 
          nodeFolder.ToString(),
          cancellationToken: cancellationToken)).StandardOutput.Trim();

        // Then - fraud
        fraudResult.Should().Be("401", "Fraudowski is a fraud.");
      }

      {
        // When - non-fraud
        var nonFraudResult = (await ReadAsync(nodeExe.ToString(), "../non_fraud_app", 
          nodeFolder.ToString(),
          cancellationToken: cancellationToken)).StandardOutput.Trim();

        // Then - non-fraud
        nonFraudResult.Should().Be("200", "Stefanowska is not a fraud");
      }
    }
    finally
    {
      //Cleanup
      await Stop(StubRunnerContainerName, cancellationToken);
    }
  }

  /// <summary>
  /// Uruchamia stub runnera w tle.
  /// </summary>
  /// <param name="cancellationToken"></param>
  private static void StartStubRunnerAsDockerContainer(CancellationToken cancellationToken)
  {
    _ = RunAsync("docker",
      " run --rm" +
      $" --name {StubRunnerContainerName}" +
      $" -e \"STUBRUNNER_IDS={StubRunnerIds}\"" +
      " -e \"STUBRUNNER_STUBS_MODE=LOCAL\"" +
      " -p \"9876:9876\"" +
      $" -e \"STUBRUNNER_REPOSITORY_ROOT={StubRunnerRepositoryRoot}\"" +
      $" -p \"{StubRunnerPort}:{StubRunnerPort}\" " +
      $" -v \"{RelevantPaths.SpringCloudContractOutputDir}:/scc_output:ro\"" +
      $" {ImageName}",
      cancellationToken: cancellationToken);
  }

  /// <summary>
  /// Instaluje Node'a. Używa skryptu mavenowego,
  /// będącego częścią kodu konsumenta node'owego.
  /// </summary>
  /// <param name="cancellationToken"></param>
  private static async Task InstallAndConfigureNode(CancellationToken cancellationToken)
  {
    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
    {
      await RunAsync(
        "powershell.exe", "./mvnw.cmd install", cancellationToken: cancellationToken);
    }
    else
    {
      await RunAsync("mvn", "install", cancellationToken: cancellationToken);
    }
  }

  /// <summary>
  /// Bardzo prymitywny mechanizm zatrzymywania kontenera
  /// jeśli ten jest już uruchomiony. Na nasze potrzeby wystarczy.
  /// </summary>
  private static async Task StopIfAlreadyRunning(string containerName, CancellationToken cancellationToken)
  {
    var dockerPsOutput = (await ReadAsync("docker", "ps", cancellationToken: cancellationToken)).StandardOutput;
    if (dockerPsOutput.Contains(containerName))
    {
      await Stop(containerName, cancellationToken);
    }
  }

  /// <summary>
  /// Zatrzymuje kontener
  /// </summary>
  private static async Task Stop(string stubRunnerContainerName, CancellationToken cancellationToken)
  {
    await RunAsync("docker", $"stop {stubRunnerContainerName}", cancellationToken: cancellationToken);
  }
}