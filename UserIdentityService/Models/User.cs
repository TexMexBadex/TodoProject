using Microsoft.AspNetCore.Identity;

namespace UserIdentityService.Models
{
  public class User : IdentityUser
  {
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
  }
}
