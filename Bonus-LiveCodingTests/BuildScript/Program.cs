using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static DotnetExeCommandLineBuilder.DotnetExeCommands;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string configuration = "Release";

var slnDirectory = AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + DirectoryName("Src");
slnDirectory.SetAsCurrentDirectory();

Target("clean", () => { Run("dotnet", Clean().Configuration(configuration)); });
Target("build", () => { Run("dotnet", Build().Configuration(configuration)); });
Target("test", DependsOn("build"), () =>
{
  Run("dotnet", Test().Configuration(configuration).NoBuild());
});

Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);