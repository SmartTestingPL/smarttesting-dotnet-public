using System;
using System.Linq;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static BuildScript.TargetNames;
using static BuildScript.Tools;
using static Bullseye.Targets;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

var srcPath = 
  AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + 
  DirectoryName("Src");
srcPath.SetAsCurrentDirectory();
var packagesDirectory = RelativeDirectoryPath("05-02-Packages");
var cdcDirectory = RelativeDirectoryPath("05-03-Cdc");
var chaosDirectory = RelativeDirectoryPath("05-04-Chaos");
var performanceDirectory = RelativeDirectoryPath("05-05-Performance");
var moduleDirectoryNames = new[]
{
  packagesDirectory,
  cdcDirectory,
  chaosDirectory,
  performanceDirectory
};

DefineStandardTargets(packagesDirectory, srcPath);
DefineCdcTargets(cdcDirectory, srcPath);
DefineChaosTargets(chaosDirectory, srcPath);
DefinePerformanceTargets(performanceDirectory, srcPath);

Target("clean", DependsOn(moduleDirectoryNames.Select(CleanTargetName).ToArray()));
Target("build", DependsOn(moduleDirectoryNames.Select(BuildTargetName).ToArray()));
Target("test", DependsOn(moduleDirectoryNames.Select(TestTargetName).ToArray()));
Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);

static void DefinePerformanceTargets(
  RelativeDirectoryPath performanceDirectory,
  AbsoluteDirectoryPath srcPath)
{
  var slnPath = srcPath + performanceDirectory;
  var testTargetName = TestTargetName(performanceDirectory);

  var cleanTargetName = CleanTargetName(performanceDirectory);
  Target(cleanTargetName, () => { DotnetClean(slnPath); });

  var buildTargetName = BuildTargetName(performanceDirectory);
  Target(buildTargetName, () => { DotNetBuild(slnPath); });

  Target(testTargetName, DependsOn(buildTargetName), () =>
  {
    DotNetTest(slnPath);
    DotNetRun(slnPath + DirectoryName("Benchmarks"));
  });
}

static void DefineCdcTargets(
  RelativeDirectoryPath cdcDirectory,
  AbsoluteDirectoryPath srcPath)
{
  var cleanTargetName = CleanTargetName(cdcDirectory);
  var cdcAbsolutePath = srcPath + cdcDirectory;
  var producerPath = cdcAbsolutePath + DirectoryName("05-03-01-Producer");
  var consumerPath = cdcAbsolutePath + DirectoryName("05-03-02-node-consumer");
  var demonstrationScriptPath = cdcAbsolutePath + DirectoryName("Scripts") + DirectoryName("RunContractTests");
  var buildDirectoryCreatedByDemonstrationScript
    = demonstrationScriptPath + DirectoryName("build");
  Target(cleanTargetName, () =>
  {
    DotNetClean(producerPath);
    DotNetClean(demonstrationScriptPath);
    Maven("clean", consumerPath);
    if (buildDirectoryCreatedByDemonstrationScript.Exists())
    {
      buildDirectoryCreatedByDemonstrationScript.Delete(true);
    }
  });

  var buildTargetName = BuildTargetName(cdcDirectory);
  Target(buildTargetName, () =>
  {
    DotNetBuild(producerPath);
    DotNetBuild(demonstrationScriptPath);
    Maven("install", consumerPath);
  });

  var testTargetName = TestTargetName(cdcDirectory);
  Target(testTargetName, DependsOn(buildTargetName), () =>
  {
    DotNetTest(producerPath);
    DotNetRun(demonstrationScriptPath);
  });
}

static void DefineStandardTargets(
  RelativeDirectoryPath moduleDirectory,
  AbsoluteDirectoryPath srcPath)
{
  var slnPath = srcPath + moduleDirectory;
  var testTargetName = TestTargetName(moduleDirectory);

  var cleanTargetName = CleanTargetName(moduleDirectory);
  Target(cleanTargetName, () => { DotnetClean(slnPath); });

  var buildTargetName = BuildTargetName(moduleDirectory);
  Target(buildTargetName, () => { DotNetBuild(slnPath); });

  Target(testTargetName, DependsOn(buildTargetName), () => { DotNetTest(slnPath); });
}

static void DefineChaosTargets(
  RelativeDirectoryPath moduleDirectory,
  AbsoluteDirectoryPath srcPath)
{
  var slnPath = srcPath + moduleDirectory;
  var testTargetName = TestTargetName(moduleDirectory);

  var cleanTargetName = CleanTargetName(moduleDirectory);
  Target(cleanTargetName, () => { DotnetClean(slnPath); });

  var buildTargetName = BuildTargetName(moduleDirectory);
  Target(buildTargetName, () => { DotNetBuild(slnPath); });

  Target(testTargetName, DependsOn(buildTargetName), () =>
  {
    Console.WriteLine("============================================================");
    Console.WriteLine("============================================================");
    Console.WriteLine("Tests from this module need to be ran manually (see READMEs)");
    Console.WriteLine("============================================================");
    Console.WriteLine("============================================================");
  });
}
  
