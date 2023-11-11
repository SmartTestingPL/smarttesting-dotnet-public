using System;

namespace UnitTests;

/// <summary>
/// Atrybut wskazujący, że dany test musi zostać zrefaktorowany w ramach pracy domowej.
/// </summary>
public class HomeworkAttribute : Attribute
{
  /// <param name="text">opis co trzeba zrobić w pracy domowej</param>
  public HomeworkAttribute(string text)
  {
  }
}