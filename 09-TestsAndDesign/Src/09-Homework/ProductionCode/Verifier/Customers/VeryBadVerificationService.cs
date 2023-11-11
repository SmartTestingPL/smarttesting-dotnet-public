using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductionCode.Verifier.Customers;

public static class VeryBadVerificationService
{
  public static async Task<bool> RunHeavyQueriesToDatabaseFromStaticMethod()
  {
    // Metoda odpalająca ciężkie zapytania do bazy danych i ściągająca
    // pół internetu.
    try
    {
      await Task.Delay(TimeSpan.FromSeconds(10));
    }
    catch (ThreadInterruptedException e)
    {
      Console.WriteLine(e.ToString());
    }

    return true;
  }
}