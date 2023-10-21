namespace BuildScript;

/// <summary>
/// Klasa pomocnicza, by uniknąć części "magicznych napisów".
/// </summary>
internal static class Targets
{
  public const string RunScriptAnalyzer = "run-script-analyzer";
  public const string InstallPester = "install-pester";
  public const string RunPester = "run-pester";
  public const string GenerateDocs = "generate-docs";
  public const string InstallScriptAnalyzer = "install-script-analyzer";
}