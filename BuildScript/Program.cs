using System;
using System.Linq;
using System.Threading.Tasks;
using AtmaFileSystem;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static DotnetExeCommandLineBuilder.DotnetExeCommands;
using static SimpleExec.Command;

namespace BuildScript;

public static class Program
{
  public static async Task Main(string[] args)
  {
    const string configuration = "Release";
    var repositoryRoot = await Git.CurrentRepositoryPath();
    var moduleRoots = new[]
    {
      RelativeDirectoryPath("00-App"),
      RelativeDirectoryPath("01-IntroductionToTesting"),
      RelativeDirectoryPath("02-UnitTesting"),
      RelativeDirectoryPath("03-IntegrationTesting"),
      RelativeDirectoryPath("04-EndToEndTesting"),
      RelativeDirectoryPath("05-Architecture"),
      RelativeDirectoryPath("06-Automation"),
      RelativeDirectoryPath("07-GoingProduction"),
      RelativeDirectoryPath("08-Advanced"),
      RelativeDirectoryPath("09-TestsAndDesign"),
      RelativeDirectoryPath("Bonus-LiveCodingTests"),
    };

    foreach (var moduleRoot in moduleRoots)
    {
      var relativeBuildScriptPath = moduleRoot + DirectoryName("BuildScript");
      var absoluteBuildScriptPath = repositoryRoot + relativeBuildScriptPath;
      var cleanTargetName = CleanTargetName(moduleRoot);
      var buildTargetName = BuildTargetName(moduleRoot);
      var testTargetName = TestTargetName(moduleRoot);

      Target(cleanTargetName, () =>
      {
        Run("dotnet", 
          Run()
            .Configuration(configuration)
            .AppArguments("clean"), 
          absoluteBuildScriptPath.ToString());
        Run("dotnet", Clean().Configuration(configuration), 
          absoluteBuildScriptPath.ToString());
      });

      Target(buildTargetName, () =>
      {
        Run("dotnet", Run().Configuration(configuration).AppArguments("build"), 
          absoluteBuildScriptPath.ToString());
      });

      Target(testTargetName, DependsOn(buildTargetName), () =>
      {
        Run("dotnet", Run().Configuration(configuration).AppArguments("test"), 
          absoluteBuildScriptPath.ToString());
      });
    }

    var allCleanTargets = moduleRoots.Select(CleanTargetName).ToArray();
    var allBuildTargets = moduleRoots.Select(BuildTargetName).ToArray();
    var allTestTargets = moduleRoots.Select(TestTargetName).ToArray();

    Target("clean", DependsOn(allCleanTargets));
    Target("build", DependsOn(allBuildTargets));
    Target("test", DependsOn(allTestTargets));

    Target("default", DependsOn("test"), () =>
    {
      Console.WriteLine("========================================");
    });

    await RunTargetsAndExitAsync(args);
  }

  private static string BuildTargetName(RelativeDirectoryPath relativeBuildScriptPath)
  {
    return $"build_{relativeBuildScriptPath}";
  }

  private static string CleanTargetName(RelativeDirectoryPath relativeBuildScriptPath)
  {
    return $"clean_{relativeBuildScriptPath}";
  }

  private static string TestTargetName(RelativeDirectoryPath relativeBuildScriptPath)
  {
    return $"test_{relativeBuildScriptPath}";
  }

}