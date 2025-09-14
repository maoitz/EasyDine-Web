using Microsoft.AspNetCore.Mvc;
using EasyDine.Web.Services;

namespace EasyDine.Web.Controllers;

public class MenuController : Controller
{
    private readonly IApiClient _api;
    public MenuController(IApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var lunch  = await _api.GetMenuItemsAsync(menu: "Lunch",  ct: HttpContext.RequestAborted);
        var dinner = await _api.GetMenuItemsAsync(menu: "Dinner", ct: HttpContext.RequestAborted);
        var drinks = await _api.GetMenuItemsAsync(menu: "Drinks", ct: HttpContext.RequestAborted); // none yet (OK)

        var vm = new MenuPageVm
        {
            Sections = new()
            {
                new MenuSectionVm { Id = "lunch",  Title = "Lunch",  Items = lunch.ToList()  },
                new MenuSectionVm { Id = "dinner", Title = "Dinner", Items = dinner.ToList() },
                new MenuSectionVm { Id = "drinks", Title = "Drinks", Items = drinks.ToList() },
            }
        };
        return View(vm);
    }

}