using System;
using AtmaFileSystem;
using SimpleExec;

namespace BuildScript
{
  internal static class Tools
  {
    const string Configuration = "Release";

    public static void Maven(string mavenTarget, AbsoluteDirectoryPath workingDirectory)
    {
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        Command.Run(
          "powershell",
          $"./mvnw.cmd {mavenTarget}",
          workingDirectory: workingDirectory.ToString());
      }
      else
      {
        Command.Run(
          "mvn",
          mavenTarget,
          workingDirectory: workingDirectory.ToString());
      }
    }

    public static void DotNetClean(AbsoluteDirectoryPath workingDirectory)
    {
      Command.Run("dotnet", $"clean -c {Configuration}", workingDirectory.ToString());
    }

    public static void DotNetRun(AbsoluteDirectoryPath workingDirectory)
    {
      Command.Run("dotnet", $"run -c {Configuration} --no-build", workingDirectory.ToString());
    }

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
}