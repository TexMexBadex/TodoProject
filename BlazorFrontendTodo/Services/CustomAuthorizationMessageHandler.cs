using BlazorFrontendTodo.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

public class CustomAuthorizationMessageHandler : DelegatingHandler //DelegatingHandler: Bruges til at manipulere HTTP-anmodninger og -svar. I dette tilfælde bruges det til at tilføje JWT-token til hver anmodning.
{
  private readonly IAuthService _authService;

  public CustomAuthorizationMessageHandler(IAuthService authService)
  {
    _authService = authService;
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var token = await _authService.GetTokenAsync();

    if (!string.IsNullOrEmpty(token))
    {
      // Tilføjer JWT-token til Authorization headeren på HTTP-anmodningen
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // Sender anmodningen videre til den næste handler i kæden eller til netværkslaget
    return await base.SendAsync(request, cancellationToken); 

    //Når vi bruger base.SendAsync(request, cancellationToken) i CustomAuthorizationMessageHandler,
    //kalder vi den SendAsync-metode, der er defineret i basisklassen (DelegatingHandler).
    //Dette betyder, at anmodningen sendes videre til den næste handler i kæden af DelegatingHandler-objekter,
    //eller hvis der ikke er flere handlers i kæden, til selve netværkslaget, hvor anmodningen faktisk sendes over HTTP.
  }
}
