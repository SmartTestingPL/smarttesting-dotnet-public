using BenchmarkDotNet.Running;

namespace Benchmarks;

/// <summary>
/// Klasa pomocnicza do uruchamiania testów microbenchmarkowych z IDE.
/// </summary>
class RunBenchmarksFromIde
{
  /// <summary>
  /// Uruchamia benchmark
  /// </summary>
  static void Main(string[] args)
  {
    BenchmarkRunner.Run<CustomerVerifierBenchmarks>();
  }
}