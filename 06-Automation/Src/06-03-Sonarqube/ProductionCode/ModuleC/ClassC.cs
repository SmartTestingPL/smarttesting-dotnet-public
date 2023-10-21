using ProductionCode.ModuleA;
using ProductionCode.ModuleB;

namespace ProductionCode.ModuleC;

/// <summary>
/// Kod wykorzystany na slajdzie do wizualizacji wzajemnej zależności modułów.
/// Klasa ClassC z modułu C korzysta z modułów A oraz B.
/// </summary>
public class ClassC
{
  public readonly ClassA ClassA;
  public readonly ClassB ClassB;

  public ClassC(ClassA classA, ClassB classB)
  {
    ClassA = classA;
    ClassB = classB;
  }
}