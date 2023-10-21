using ProductionCode.ModuleA;
using ProductionCode.ModuleC;

namespace ProductionCode.ModuleB;

/// <summary>
/// Kod wykorzystany na slajdzie do wizualizacji wzajemnej zależności modułów.
/// Klasa ClassB z modułu B korzysta z modułów A oraz C.
/// </summary>
public class ClassB
{
  public readonly ClassA ClassA;
  public readonly ClassC ClassC;

  public ClassB(ClassA classA, ClassC classC)
  {
    ClassA = classA;
    ClassC = classC;
  }
}