using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Przykład źle zaprojektowanego serwisu używającego metody statycznej
/// do realizacji ciężkich operacji, np. zapytań bazodanowych albo zapytań HTTP.
/// </summary>
public static class VeryBadVerificationService
{
  public static async Task<bool> RunHeavyQueriesToDatabaseFromStaticMethod(CancellationToken cancellationToken)
  {
    // Metoda odpalająca ciężkie zapytania do bazy danych i ściągająca pół internetu.
    try
    {
      await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
    }
    catch (TaskCanceledException e)
    {
      Console.WriteLine(e.ToString());
    }

    return true;
  }
}