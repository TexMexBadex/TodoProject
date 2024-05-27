using BlazorFrontendTodo.Services.Interfaces;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace BlazorFrontendTodo.Services;

public class AuthService : IAuthService
{
  private const string TokenKey = "authToken"; // Nøglen til token i session storage
  private readonly IJSRuntime _jsRuntime; // JavaScript runtime til at interagere med browserens session storage
  private readonly IHttpClientFactory _httpClientFactory;

  public AuthService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory)
  {
    _jsRuntime = jsRuntime;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<string> GetTokenAsync()
  {
    return await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", TokenKey);
  }

  public async Task SetTokenAsync(string token)
  {
    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", TokenKey, token);
  }

  public async Task RemoveTokenAsync()
  {
    await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
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

  public async Task<string> GetUserEmailAsync()
  {
    var httpClient = _httpClientFactory.CreateClient("userApi");
    var token = await GetTokenAsync();

    if (string.IsNullOrEmpty(token))
    {
      throw new Exception("User is not authenticated");
    }


    var response = await httpClient.GetAsync("api/auth/email");
    response.EnsureSuccessStatusCode();

    var email = await response.Content.ReadAsStringAsync();
    return email;
  }


}