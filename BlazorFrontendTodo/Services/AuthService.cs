using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorFrontendTodo.Models;
using BlazorFrontendTodo.Services.Interfaces;
using Microsoft.JSInterop;

public class AuthService : IAuthService
{ 
  private const string TokenKey = "authToken"; // Nøglen til token i local storage
  private readonly IJSRuntime _jsRuntime; // JavaScript runtime til at interagere med browserens local storage
  private readonly IHttpClientFactory _httpClientFactory;


  public AuthService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory)
  {
    _jsRuntime = jsRuntime;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<string> GetTokenAsync()
  {
    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
  }

  public async Task SetTokenAsync(string token)
  {
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
  }

  public async Task RemoveTokenAsync()
  {
    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
  }

  public async Task<string> GetUserIdAsync()
  {
    var httpClient = _httpClientFactory.CreateClient("userApi");

    var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/userid");

    var response = await httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    var userId = await response.Content.ReadAsStringAsync();
    return userId;
  }
}
