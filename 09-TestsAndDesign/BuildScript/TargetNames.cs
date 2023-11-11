using AtmaFileSystem;

namespace BuildScript;

internal static class TargetNames
{
  public static string TestTargetName(RelativeDirectoryPath relativeDirectoryPath)
  {
    return $"test_{relativeDirectoryPath}";
  }

  public static string BuildTargetName(RelativeDirectoryPath relativeDirectoryPath)
  {
    return $"build_{relativeDirectoryPath}";
  }

  public static string CleanTargetName(RelativeDirectoryPath relativeDirectoryPath)
  {
    return $"clean_{relativeDirectoryPath}";
  }
}