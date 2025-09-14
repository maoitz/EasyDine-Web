using EasyDine.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyDine.Web.Filters;

public sealed class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        var tokens = ctx.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
        if (string.IsNullOrWhiteSpace(tokens.AccessToken))
            ctx.Result = new RedirectToActionResult("Login", "Admin", null);
    }
}