using FluentResults;
using Microsoft.AspNetCore.Identity;
using UserIdentityService.Data;
using UserIdentityService.Interfaces;
using UserIdentityService.Models;
using BC = BCrypt.Net.BCrypt;

namespace UserIdentityService.Features
{
  public class UserService : IUserService
  {
    private readonly UserContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(UserContext context, IPasswordHasher<User> passwordHasher)
    {
      _context = context;
      _passwordHasher = passwordHasher;
    }
    //todo: take a request model, map it to the user-model
    /// <summary>
    /// Creates a user, hashes the password before saving
    /// </summary>
    /// <param name="username"> the given username </param>
    /// <param name="password"> the given password </param>
    /// <param name="name"> the name of the user </param>
    /// <param name="email"> the email of the user </param>
    public Result Register(string username, string password, string name, string email)
    {
      if (_context.Users.Any(u => u.Username == username))
      {
        return Result.Fail($"Username '{username}' is already taken.");
      }

      try
      {
        var passwordHash = BC.EnhancedHashPassword(password);
        var user = new User
        {
          Username = username,
          PasswordHash = passwordHash,
          Name = name,
          Email = email
        };

        //todo: unit of work??
        _context.Users.Add(user);
        _context.SaveChanges();

        return Result.Ok();
      }
      catch (Exception ex)
      {
        return Result.Fail($"Something went wrong when trying to create a user. Exception: {ex.Message}");

      }

    }
  }
}

