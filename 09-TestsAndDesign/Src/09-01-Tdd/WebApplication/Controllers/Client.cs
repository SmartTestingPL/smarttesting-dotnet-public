namespace WebApplication.Controllers;

public class Client
{
  public readonly bool HasDebt;

  public Client(bool hasDebt)
  {
    HasDebt = hasDebt;
  }
}