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

    // Initialiserer en instans af JwtSecurityTokenHandler til at oprette og validere JWT'er.
    var tokenHandler = new JwtSecurityTokenHandler();

    // Henter den hemmelige nøgle fra konfigurationsfilerne og koder den til en byte-array ved hjælp af ASCII-kodning.
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

    // Opretter en token descriptor, der beskriver indholdet og signaturen af JWT'en.
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      // Opretter en ClaimsIdentity med brugerens ID og email som claims.
      Subject = new ClaimsIdentity(new[]
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
      }),
      // Sætter udløbstidspunktet for tokenen til 7 dage fra nu.
      Expires = DateTime.UtcNow.AddDays(7),
      // Definerer signeringsoplysningerne, herunder den hemmelige nøgle og signeringsalgoritmen (HMAC SHA256).
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    // Opretter en sikkerhedstoken baseret på tokenDescriptor.
    var token = tokenHandler.CreateToken(tokenDescriptor);

    // Konverterer tokenen til en streng, som kan sendes til klienten.
    var tokenString = tokenHandler.WriteToken(token);

    // Returnerer tokenen som en del af HTTP-responsen.
    return Ok(new { Token = tokenString });

  }
}
