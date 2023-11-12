using Microsoft.AspNetCore.Mvc;

namespace WebAppTests;

public abstract class PyramidTests
{
}

[ApiController]
[Route("[controller]")]
internal class UserController
{
  private readonly UserRepository _repo;

  internal UserController(UserRepository repo)
  {
    _repo = repo;
  }

  [HttpGet("/user/{id}")]
  internal User User(string id)
  {
    return _repo.FindUserById(id);
  }
}

internal class UserRepository
{
  private readonly DatabaseAccessor _accessor;

  internal UserRepository(DatabaseAccessor accessor)
  {
    _accessor = accessor;
  }

  internal User FindUserById(string id)
  {
    return _accessor.ExecuteSql("...");
  }
}

internal class User
{

}

internal class DatabaseAccessor
{
  internal User ExecuteSql(string s)
  {
    return null;
  }
}