namespace EasyDine.Web.Services;

public interface ITokenStore
{
    string? AccessToken { get; set; }
    void Clear();
}