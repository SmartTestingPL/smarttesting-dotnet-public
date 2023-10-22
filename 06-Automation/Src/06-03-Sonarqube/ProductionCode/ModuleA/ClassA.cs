using ProductionCode.ModuleB;
using ProductionCode.ModuleC;

namespace ProductionCode.ModuleA;

/// <summary>
/// Kod wykorzystany na slajdzie do wizualizacji wzajemnej zależności modułów.
/// Klasa ClassA z modułu A korzysta z modułów B oraz C.
/// </summary>
public class ClassA
{
  public readonly ClassB ClassB;
  public readonly ClassC ClassC;

  public ClassA(ClassB classB, ClassC classC)
  {
    ClassB = classB;
    ClassC = classC;
  }
}