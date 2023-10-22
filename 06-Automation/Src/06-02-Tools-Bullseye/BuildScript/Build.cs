using System;
using System.IO;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

const string buildConfigurationSwitch = "-c Release";

// Ścieżka do folderu w którym znajduje się sln
var targetSlnDirectory 
  = AbsoluteDirectoryPath.OfThisFile().ParentDirectory().Value() + DirectoryName("Src");

Console.WriteLine("Target solution directory: " + targetSlnDirectory);

// Ustawiamy katalog budowanego slna jako katalog roboczy.
// Katalg roboczy można też przekazać do poleceń SimpleExec
// jako parametr.
targetSlnDirectory.SetAsCurrentDirectory();

// definicje celów używające API Bullseye i SimpleExec.
Target("clean", () =>
{
  Run("dotnet", $"clean {buildConfigurationSwitch}");
});
      
Target("build", () =>
{
  Run("dotnet", $"build {buildConfigurationSwitch}");
});

//Za pomocą metody DependsOn() określamy zadania, od których zależymy
Target("test", DependsOn("build"), () =>
{
  Run("dotnet", $"test --no-build {buildConfigurationSwitch}");
});

Target("default", DependsOn("test"));
await RunTargetsAndExitAsync(args);
