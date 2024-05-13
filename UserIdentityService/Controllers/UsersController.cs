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

    

  }
}
