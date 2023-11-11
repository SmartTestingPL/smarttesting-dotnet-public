using Microsoft.AspNetCore.Mvc;
using WebApplication.Logic;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly UserRepository _repo;

  public UserController(UserRepository repo)
  {
    _repo = repo;
  }

  [HttpGet("user/{id}")]
  public IActionResult GetUser(string id)
  {
    return Ok(_repo.FindUserById(id));
  }
}