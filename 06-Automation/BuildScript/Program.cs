using System.Linq;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string configuration = "Release";

var srcDirectory = 
  AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + 
  DirectoryName("Src");
srcDirectory.SetAsCurrentDirectory();
var pesterDirectoryName = RelativeDirectoryPath("06-02-Tools-Pester");
var sonarQubeDirectoryName = RelativeDirectoryPath("06-03-Sonarqube");
var standardBuiltFolders = new[]
{
  RelativeDirectoryPath("06-02-Tools-Bullseye") + DirectoryName("Src"),
  RelativeDirectoryPath("06-02-Tools-Bullseye") + DirectoryName("BuildScript"),
  RelativeDirectoryPath("06-02-Tools-Cake") + DirectoryName("Src"),
  RelativeDirectoryPath("06-02-Tools-Nuke"),
  pesterDirectoryName,
  sonarQubeDirectoryName,
};

foreach (var relativeDirectoryPath in standardBuiltFolders)
{
  DefineCleanAndBuildTargets(relativeDirectoryPath, srcDirectory, configuration);  
}

Target(
  TestTargetName(pesterDirectoryName), 
  DependsOn(BuildTargetName(pesterDirectoryName)), 
  () =>
  {
    Run("dotnet", $"run --no-build -c {configuration}", 
      (srcDirectory + pesterDirectoryName + DirectoryName("BuildScript")).ToString());
  });

Target(
  TestTargetName(sonarQubeDirectoryName),
  DependsOn(BuildTargetName(sonarQubeDirectoryName)),
  () =>
  {
    Run("dotnet", $"test --no-build -c {configuration}", (srcDirectory + sonarQubeDirectoryName).ToString());
  });

Target("clean", DependsOn(standardBuiltFolders.Select(CleanTargetName).ToArray()));
Target("build", DependsOn(standardBuiltFolders.Select(BuildTargetName).ToArray()));
Target("test", DependsOn(
  "build", 
  TestTargetName(pesterDirectoryName),
  TestTargetName(sonarQubeDirectoryName)));
Target("default", DependsOn("test"));

await RunTargetsAndExitAsync(args);

static void DefineCleanAndBuildTargets(
  RelativeDirectoryPath relativeDirectoryPath,
  AbsoluteDirectoryPath srcDirectory,
  string configuration)
{
  var slnDirectory = (srcDirectory + relativeDirectoryPath).ToString();
  var cleanTargetName = CleanTargetName(relativeDirectoryPath);
  var buildTargetName = BuildTargetName(relativeDirectoryPath);

  Target(cleanTargetName, () => { Run("dotnet", $"clean -c {configuration}", slnDirectory); });

  Target(buildTargetName, () => { Run("dotnet", $"build -c {configuration}", slnDirectory); });
}

static string TestTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"test_{relativeDirectoryPath}";
}

static string BuildTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"build_{relativeDirectoryPath}";
}

static string CleanTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"clean_{relativeDirectoryPath}";
}
