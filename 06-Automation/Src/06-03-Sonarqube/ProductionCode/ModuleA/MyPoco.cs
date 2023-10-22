namespace ProductionCode.ModuleA;

/// <summary>
/// Klasa, która jest niezwiązana z żadnym szkieletem aplikacyjnym czy
/// biblioteką (POCO - Plain Old CLR Object).
/// </summary>
public class MyPoco
{
  public string Name { get; set; } = null!;
  public string Surname { set; get; } = null!;
  public int Age { set; get; }
}