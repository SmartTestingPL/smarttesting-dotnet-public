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

Target("clean", () =>
{
  Run("dotnet", $"clean -c {configuration}");
});

Target("build", () =>
{
  Run("dotnet", $"build -c {configuration}");
});

Target("test", DependsOn("build"), () =>
{
  Run("dotnet", $"test ./FraudVerifierTests/FraudVerifierTests.csproj -c {configuration}");
  Run("dotnet", $"test ./LoanOrdersTests/LoanOrdersTests.csproj -c {configuration}");
});
Target("default", DependsOn("test"));

await RunTargetsAndExitAsync(args);