namespace EasyDine.Web.DTOs.Admin;

public sealed class LoginResult
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? Error { get; init; }
}   