using System;
using System.IO;
using System.Threading.Tasks;
using AtmaFileSystem;
using AtmaFileSystem.IO;
using static AtmaFileSystem.AtmaFileSystemPaths;
using static BuildScript.Targets;
using static Bullseye.Targets;
using static SimpleExec.Command;
using AbsoluteDirectoryPath = AtmaFileSystem.AbsoluteDirectoryPath;

namespace BuildScript;

public static class Program
{
  private static readonly AbsoluteDirectoryPath ScriptDirectory = AbsoluteDirectoryPath.OfThisFile();
  private static readonly AbsoluteDirectoryPath SrcPath = ScriptDirectory + DirectoryName("src");
  private static readonly AbsoluteDirectoryPath DocPath = ScriptDirectory + DirectoryName("doc");
  private static readonly AbsoluteDirectoryPath TestPath = ScriptDirectory + DirectoryName("test");
  private static readonly AbsoluteDirectoryPath ToolsPath = ScriptDirectory + DirectoryName("tools");
  private static readonly AbsoluteFilePath Ps1ScriptPath = SrcPath + FileName("Script.ps1");

  public static async Task Main(string[] args)
  {
    //Ustawiamy katalog aktualnego skryptu jako roboczy:
    ScriptDirectory.SetAsCurrentDirectory();

    // generowanie dokumentacji skryptu używając polecenia Get-Help
    // i przekierowując jego wyjście do pliku.
    Target(GenerateDocs, () =>
    {
      var ps1DocPath = DocPath + FileName("Script.ps1.txt");
      if (!Directory.Exists(DocPath.ToString()))
      {
        Directory.CreateDirectory(DocPath.ToString());
      }
      PowershellCommand($"Get-Help {Ps1ScriptPath} | Out-File -FilePath {ps1DocPath}");
    });

    // Instalacja narzędzia ScriptAnalyzer - lintera do skryptów PowerShell
    Target(InstallScriptAnalyzer, () =>
    {
      BuildHelper("install-script-analyzer");
    });

    // Uruchomienie narzędzia ScriptAnalyzer na naszym skrypcie
    Target(RunScriptAnalyzer, DependsOn(InstallScriptAnalyzer), () =>
    {
      PowershellCommand($"Invoke-ScriptAnalyzer -Path '{Ps1ScriptPath}'");
    });

    // Instalacja narzędzia do testowania skryptów PowerShell - Pester
    Target(InstallPester, () =>
    {
      BuildHelper("install-pester");
    });

    // Uruchomienie testów Pestera, które przetestują nasz skrypt
    Target(RunPester, DependsOn(InstallPester), () =>
    {
      PowershellCommand("Invoke-Pester -Script './TestScript.ps1'", TestPath);
    });

    // Domyślny cel - wygeneruj dokumentację, przeskanuj skrypty i odpal testy
    Target("default", DependsOn(GenerateDocs, RunScriptAnalyzer, RunPester));

    await RunTargetsAndExitAsync(args);
  }

  /// <summary>
  /// Uruchom skrypt BuildHelper z odpowiednią komendą
  /// </summary>
  /// <param name="command">komenda do wykonania</param>
  private static void BuildHelper(string command)
  {
    PowershellScript($"{ToolsPath + FileName("BuildHelper.ps1")} {command}");
  }

  /// <summary>
  /// Uruchomienie skryptu powershellowego
  /// </summary>
  private static void PowershellScript(string scriptInvocation)
  {
    Run(PowershellCommandName(), args: $"-ExecutionPolicy Unrestricted {scriptInvocation}");
  }

  /// <summary>
  /// Uruchomienie komendy powershellowej.
  /// </summary>
  private static void PowershellCommand(string commandString)
  {
    Run(PowershellCommandName(), 
      args: $"-ExecutionPolicy Unrestricted -command \"{commandString}\"");
  }

  /// <summary>
  /// Uruchomienie komendy powershellowej z podanym folderem jako roboczym.
  /// </summary>
  private static void PowershellCommand(string commandString, AbsoluteDirectoryPath workingDirectory)
  {
    Run(PowershellCommandName(), 
      args: $"-ExecutionPolicy Unrestricted -command \"{commandString}\"", 
      workingDirectory: workingDirectory.ToString());
  }

  private static string PowershellCommandName() 
    => Environment.OSVersion.Platform == PlatformID.Win32NT ? "powershell.exe" : "pwsh";
}