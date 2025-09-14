using System.Security.Claims;
using EasyDine.Web.Services;
using EasyDine.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
    
namespace EasyDine.Web.Controllers;

public class AdminController : Controller
{
    private readonly IApiClient _api;
    private readonly ITokenStore _tokens;

    public AdminController(IApiClient api, ITokenStore tokens)
    {
        _api = api;
        _tokens = tokens;
    }
    
    [HttpGet]
    public IActionResult Login() => View(new AdminLoginVm());

    [HttpPost]
    public async Task<IActionResult> Login(AdminLoginVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _api.AdminLoginAsync(vm.Username, vm.Password, ct);
        if (!result.Success || string.IsNullOrEmpty(result.AccessToken))
        {
            vm.Error = result.Error ?? "Login failed.";
            return View(vm);
        }

        _tokens.AccessToken = result.AccessToken;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, vm.Username),
            new(ClaimTypes.Role, "Admin")
        };
        var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _tokens.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(DateTime? date, CancellationToken ct)
    {
        var vm = new DashboardVm
        {
            TotalBookings = (await _api.AdminListBookingsAsync(date, ct)).Count,
            TotalTables = (await _api.AdminListTablesAsync(ct)).Count,
            TotalMenuItems = (await _api.AdminListMenuItemsAsync(ct)).Count,
            PopularItems = await _api.AdminGetPopularItemsAsync(ct),
            RecentBookings = await _api.AdminGetRecentBookingsAsync(5, ct)
        };
        vm.PopularItemsCount = vm.PopularItems.Count;

        return View(vm);
    }

}