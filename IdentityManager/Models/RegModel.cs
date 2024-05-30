namespace IdentityManager.Models
{
  public class RegModel
  {
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string ConfirmPassword { get; set; } = string.Empty;
  }
}
