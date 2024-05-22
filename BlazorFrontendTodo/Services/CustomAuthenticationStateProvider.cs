using BlazorFrontendTodo.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IAuthService _authService; // Reference til AuthService

  public CustomAuthenticationStateProvider(IAuthService authService)
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

  /// <summary>
  /// Denne metode bruges til at kontrollere, om en JWT-token er gyldig
  /// Skal bruges ifm opstart af program, så hvis man ikke er logget ud
  /// Fra sidste brug, kan man fortsætte "hvor man slap" med samme token
  /// hvis den ikke er udløbet
  /// </summary>
  /// <returns> true / false </returns>
  public async Task<bool> IsTokenValid()
  {
    var token = await _authService.GetTokenAsync(); //hent token fra local storage
    if (string.IsNullOrEmpty(token)) 
    {
      return false; //eksisterer den ikke, returner false
    }

    var claims = JwtParser.ParseClaimsFromJwt(token); //parse claims og hent info fra tokenen
    var expiry = claims.FirstOrDefault(c => c.Type.Equals("exp"))?.Value; //find udløbstidspunktet
    if (expiry == null)
    {
      return false; //findes det ikke, returner false
    }
    //DateTimeOffset fælles grundlag for "instantaneous time", se: https://stackoverflow.com/questions/4331189/datetime-vs-datetimeoffset
    var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry)); //parse som long, så unix-tidsstemplet kan sammenlignes med dags dato + klokkeslæt
    return expDate > DateTimeOffset.Now;
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
