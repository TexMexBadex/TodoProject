using FluentResults;

namespace UserIdentityService.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Create a user, hashes the password before saving
    /// </summary>
    /// <param name="username"> the given username </param>
    /// <param name="password"> the given password </param>
    /// <param name="name"> the name of the user </param>
    /// <param name="email"> the email of the user </param>
    /// <returns></returns>
    Result Register(string username, string password, string name, string email);
}