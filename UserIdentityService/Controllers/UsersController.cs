using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserIdentityService.Interfaces;
using UserIdentityService.Models;

namespace UserIdentityService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IUserService _userService; // Inject your user service that handles the business logic

    public UsersController(IUserService userService)
    {
      _userService = userService;
    }

    //TODO: Fix
    [HttpPost("register")]
    public ActionResult<User> Register([FromBody] User request)
    {
      var user = _userService.Register(request.Username, request.PasswordHash, request.Name, request.Email);
      if (user == null)
      {
        return BadRequest("User could not be registered.");
      }
      return Ok(user);
    }

  }
}
