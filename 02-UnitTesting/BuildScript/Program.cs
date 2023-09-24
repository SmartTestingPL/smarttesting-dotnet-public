using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string configuration = "Release";

var slnDirectory = AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + DirectoryName("Src");
slnDirectory.SetAsCurrentDirectory();

Target("clean", () => { Run("dotnet", $"clean -c {configuration}"); });
Target("build", () => { Run("dotnet", $"build -c {configuration}"); });
Target("test", DependsOn("build"), () => { Run("dotnet", $"test -c {configuration} --no-build"); });

Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);