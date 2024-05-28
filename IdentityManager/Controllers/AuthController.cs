using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IdentityManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly IConfiguration _configuration;

  public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  [HttpGet("userid")]
  public IActionResult GetUserId()
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null)
    {
      return Unauthorized();
    }

    return Ok(userId);
  }

  [HttpGet("email")]
  [Authorize]
  public IActionResult GetEmail()
  {
    var email = User.FindFirstValue(ClaimTypes.Email);
    if (email == null)
    {
      return Unauthorized("Email not found in token.");
    }

    return Ok(email);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegModel model)
  {
    if (string.IsNullOrWhiteSpace(model.Email))
    {
      return BadRequest("Email is required.");
    }

    if (string.IsNullOrWhiteSpace(model.Password))
    {
      return BadRequest("Password is required.");

    }

    if (model.Password != model.ConfirmPassword)
    {
      return BadRequest("Passwords do not match.");
    }

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user != null)
    {
      return BadRequest("A user with this email already exists.");
    }

    user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await _userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
    {
      return Ok("User created successfully");
    }

    var errors = result.Errors.Select(e => e.Description).ToList();
    return BadRequest(errors);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginModel model)
  {
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
    {
      return BadRequest("Invalid email or password.");
    }

    if (!await _userManager.CheckPasswordAsync(user, model.Password))
    {
      return BadRequest("Invalid email or password.");
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
      }),
      Expires = DateTime.UtcNow.AddDays(7),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);


    var cookieOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      Expires = DateTime.UtcNow.AddDays(7),
      SameSite = SameSiteMode.Strict
    };

    Response.Cookies.Append("authToken", tokenString, cookieOptions);

    return Ok(new { Token = tokenString });
  }
}
