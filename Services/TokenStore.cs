namespace EasyDine.Web.Services;

public sealed class TokenStore : ITokenStore
{
    public string? AccessToken { get; set; }
    public void Clear() => AccessToken = null;
}