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

var moduleDirectoryNames = new[]
{
  RelativeDirectoryPath("09-01-Tdd"),
  RelativeDirectoryPath("09-02-Pyramid"),
  RelativeDirectoryPath("09-03-BadPlugin"),
  RelativeDirectoryPath("09-03-BadTests"),
  RelativeDirectoryPath("09-04-Legacy"),
  RelativeDirectoryPath("09-Homework"),
};
foreach (var moduleDirectoryName in moduleDirectoryNames)
{
  DefineStandardTargets(moduleDirectoryName, srcPath);
}

Target("clean", DependsOn(moduleDirectoryNames.Select(CleanTargetName).ToArray()));
Target("build", DependsOn(moduleDirectoryNames.Select(BuildTargetName).ToArray()));
Target("test", DependsOn(moduleDirectoryNames.Select(TestTargetName).ToArray()));
Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);


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
