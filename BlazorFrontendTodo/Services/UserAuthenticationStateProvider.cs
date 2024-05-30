using System.Security.Claims;
using System.Text.Json;
using BlazorFrontendTodo.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorFrontendTodo.Services;

public class UserAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IAuthService _authService; 

  public UserAuthenticationStateProvider(IAuthService authService)
  {
    _authService = authService;
  }

  // Hent nuværende autentificeringstilstand
  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var token = await _authService.GetTokenAsync(); // Hent token fra AuthService
    var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
    var user = new ClaimsPrincipal(identity); // Opret en bruger baseret på token

    return new AuthenticationState(user); // Returner autentificeringstilstanden
  }

  // Underret systemet om, at brugeren er autentificeret
  public void NotifyUserAuthentication(string token)
  {
    var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
    var user = new ClaimsPrincipal(identity);
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user))); // Underret om ændret autentificeringstilstand
  }

  // Underret systemet om, at brugeren er logget ud
  public void NotifyUserLogout()
  {
    var identity = new ClaimsIdentity();
    var user = new ClaimsPrincipal(identity);
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user))); // Underret om ændret autentificeringstilstand
  }


}

public static class JwtParser
{
  // Parse claims fra JWT-token
  public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
  {
    var payload = jwt.Split('.')[1]; // Hent payload-delen af token
    var jsonBytes = ParseBase64WithoutPadding(payload);
    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

    return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
  }

  // Parse base64-streng uden padding
  private static byte[] ParseBase64WithoutPadding(string base64)
  {
    switch (base64.Length % 4)
    {
      case 2: base64 += "=="; break;
      case 3: base64 += "="; break;
    }
    return Convert.FromBase64String(base64);
  }
}