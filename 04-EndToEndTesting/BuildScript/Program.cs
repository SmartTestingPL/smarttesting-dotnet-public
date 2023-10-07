using System;
using System.Linq;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string configuration = "Release";

var srcDirectory = AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value().AddDirectoryName("Src");
srcDirectory.SetAsCurrentDirectory();

var standardBuiltFolders = new[]
{
  RelativeDirectoryPath("04-01-RestBasedE2E"),
  RelativeDirectoryPath("04-02-GuiBasedE2E"),
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


static void DefineStandardTargets(
  RelativeDirectoryPath relativeDirectoryPath,
  AbsoluteDirectoryPath srcDirectory,
  string configuration)
{
  var slnDirectory = (srcDirectory + relativeDirectoryPath).ToString();
  var cleanTargetName = CleanTargetName(relativeDirectoryPath);
  var buildTargetName = BuildTargetName(relativeDirectoryPath);
  var testTargetName = TestTargetName(relativeDirectoryPath);

  Target(cleanTargetName, () => { Run("dotnet", $"clean -c {configuration}", slnDirectory); });

  Target(buildTargetName, () => { Run("dotnet", $"build -c {configuration}", slnDirectory); });

  Target(testTargetName, DependsOn(buildTargetName), () =>
  {
    Console.WriteLine("============================================================");
    Console.WriteLine("============================================================");
    Console.WriteLine("Tests from this module need to be ran manually (see READMEs)");
    Console.WriteLine("============================================================");
    Console.WriteLine("============================================================");
  });
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