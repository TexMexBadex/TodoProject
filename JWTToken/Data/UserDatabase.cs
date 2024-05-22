namespace JWTToken.Data
{
  public class User
  {
    public string Email { get; }
    public User(string email)
    {
      Email = email;
    }
  }
}
