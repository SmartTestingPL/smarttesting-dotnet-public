using System.Threading.Tasks;
using AtmaFileSystem;
using static SimpleExec.Command;

namespace BuildScript;

internal static class Git
{
  public static async Task<AbsoluteDirectoryPath> CurrentRepositoryPath()
  {
    return AbsoluteDirectoryPath.Value(
      (await ReadAsync("git", " rev-parse --show-toplevel")).StandardOutput.Replace("\n", ""));
  }
}