using System.Threading;
using System.Threading.Tasks;

namespace ProductionCode.Verifier.Customers;

/// <summary>
/// Klasa "wrapper" otaczająca statyczną metodę, która realizuje
/// jakieś ciężkie operacje bazodanowe.
///
/// Nie polecamy robienia takich operacji w metodzie statycznej, ale tu pokazujemy
/// jak to obejść i przetestować jeżeli z jakiegoś powodu nie da się tego zmienić
/// (np. metoda statyczna jest dostarczana przez kogoś innego).
/// </summary>
/// <see cref="VeryBadVerificationService"/>
public class VeryBadVerificationServiceWrapper
{
  public virtual Task<bool> Verify(CancellationToken cancellationToken)
  {
    return VeryBadVerificationService.RunHeavyQueriesToDatabaseFromStaticMethod(cancellationToken);
  }
}