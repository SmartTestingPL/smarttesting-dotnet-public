using AtmaFileSystem;
using SimpleExec;

namespace BuildScript;

internal static class Tools
{
  private const string Configuration = "Release";

  public static void DotNetTest(AbsoluteDirectoryPath workingDirectory)
  {
    Command.Run("dotnet", $"test -c {Configuration} --no-build", workingDirectory.ToString());
  }

  public static void DotNetBuild(AbsoluteDirectoryPath workingDirectory)
  {
    Command.Run("dotnet", $"build -c {Configuration}", workingDirectory.ToString());
  }

  public static void DotnetClean(AbsoluteDirectoryPath workingDirectory)
  {
    Command.Run("dotnet", $"clean -c {Configuration}", workingDirectory.ToString());
  }
}