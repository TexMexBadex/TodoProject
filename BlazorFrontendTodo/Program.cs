using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorFrontendTodo;
using BlazorFrontendTodo.Services;
using Microsoft.Extensions.DependencyInjection;
using BlazorFrontendTodo.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Konfigurer en standard HttpClient til Blazor WebAssembly-applikationen
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// Tilføj en HttpClient til Task API
builder.Services.AddHttpClient("taskApi", client =>
{
  client.BaseAddress = new Uri("https://localhost:7183"); // Task API base-URL
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

// Tilføj en HttpClient til User API
builder.Services.AddHttpClient("userApi", client =>
{
  client.BaseAddress = new Uri("https://localhost:7241"); // User API base-URL
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddHttpClient("unauthenticatedUserApi", client =>
{
  client.BaseAddress = new Uri("https://localhost:7241"); // User API base-URL
});

builder.Services.AddScoped<NotificationService>();

// Registrer CustomAuthenticationStateProvider og AuthService
builder.Services.AddScoped<UserAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<UserAuthenticationStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddTransient<AuthorizationMessageHandler>(); //Transient for at sikre, at handleren er stateless og "frisk" for hver HTTP-anmodning

// Tilføj autorisation services
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
