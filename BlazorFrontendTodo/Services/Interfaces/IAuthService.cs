using BlazorFrontendTodo.Models;

namespace BlazorFrontendTodo.Services.Interfaces
{
  public interface IAuthService
  {
    Task<string> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
    Task<string> GetUserIdAsync();
  }
}
