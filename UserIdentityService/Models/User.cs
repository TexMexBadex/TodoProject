namespace UserIdentityService.Models
{
  public class User
  {
    public Guid Id { get; set; } = Guid.NewGuid(); // Automatically generate a new GUID for each user
    public required string Username { get; set; }
    public required string PasswordHash { get; set; } // Storing a hash of the password
    public required string Name { get; set; }
    public required string Email { get; set; }
  }
}
