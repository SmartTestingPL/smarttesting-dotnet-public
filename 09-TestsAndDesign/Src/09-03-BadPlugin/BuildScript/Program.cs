using AtmaFileSystem;
using FluentAssertions;
using static SimpleExec.Command;
using static Bullseye.Targets;

var solutionDirectory =
  AbsoluteDirectoryPath.OfThisFile()
    .ParentDirectory().Value().ToString();

Target("default", async () =>
{
  var output = await ReadAsync("dotnet", "test UnitTests", solutionDirectory);
  output.StandardOutput.Should().NotContain("No test is available");
  output.StandardError.Should().NotContain("No test is available");
});
await RunTargetsAndExitAsync(args);

