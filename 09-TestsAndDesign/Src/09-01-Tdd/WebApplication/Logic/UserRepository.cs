namespace WebApplication.Logic;

public class UserRepository
{
  private readonly DatabaseAccessor _accessor;

  public UserRepository(DatabaseAccessor accessor)
  {
    _accessor = accessor;
  }

  public User FindUserById(string id)
  {
    return _accessor.ExecuteSql("...");
  }
}