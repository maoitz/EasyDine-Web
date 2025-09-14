using System.Net;
using System.Net.Http.Headers;
using EasyDine.Web.Services;

namespace EasyDine.Web.DelegatingHandlers;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStore _tokens;

    public AuthHeaderHandler(ITokenStore tokens) => _tokens = tokens;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(_tokens.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokens.AccessToken);

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _tokens.Clear(); // next admin request will be redirected by filter

        return response;
    }
}