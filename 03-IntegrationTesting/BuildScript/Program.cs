using System.Linq;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string configuration = "Release";

var srcDirectory = AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + DirectoryName("Src");
srcDirectory.SetAsCurrentDirectory();

var standardBuiltFolders = new[]
{
  RelativeDirectoryPath("03-02-Ioc"),
  RelativeDirectoryPath("03-03-Controller"),
  RelativeDirectoryPath("03-04-HttpClient"),
  RelativeDirectoryPath("03-05-Db"),
  RelativeDirectoryPath("03-06-Messaging"),
  RelativeDirectoryPath("03-07-Dev") + DirectoryName("03-07-01-InMem"),
  RelativeDirectoryPath("03-07-Dev") + DirectoryName("03-07-02-Runner"),
};

foreach (var relativeDirectoryPath in standardBuiltFolders)
{
  DefineStandardTargets(relativeDirectoryPath, srcDirectory, configuration);
}

Target("clean", DependsOn(standardBuiltFolders.Select(CleanTargetName).ToArray()));
Target("build", DependsOn(standardBuiltFolders.Select(BuildTargetName).ToArray()));
Target("test", DependsOn(standardBuiltFolders.Select(TestTargetName).ToArray()));
Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);

void DefineStandardTargets(
  RelativeDirectoryPath relativeDirectoryPath,
  AbsoluteDirectoryPath srcDirectory,
  string configuration)
{
  var slnDirectory = (srcDirectory + relativeDirectoryPath).ToString();
  var cleanTargetName = CleanTargetName(relativeDirectoryPath);
  var buildTargetName = BuildTargetName(relativeDirectoryPath);
  var testTargetName = TestTargetName(relativeDirectoryPath);

  Target(cleanTargetName, () =>
  {
    Run("dotnet", $"clean -c {configuration}", slnDirectory);
  });

  Target(buildTargetName, () =>
  {
    Run("dotnet", $"build -c {configuration}", slnDirectory);
  });

  Target(testTargetName, DependsOn(buildTargetName), () =>
  {
    Run("dotnet", $"test -c {configuration} --no-build", slnDirectory);
  });
}

string TestTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"test_{relativeDirectoryPath}";
}

string BuildTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"build_{relativeDirectoryPath}";
}

string CleanTargetName(RelativeDirectoryPath relativeDirectoryPath)
{
  return $"clean_{relativeDirectoryPath}";
}