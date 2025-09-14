using EasyDine.Web.Filters;
using EasyDine.Web.Services;
using EasyDine.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace EasyDine.Web.Controllers;

[AdminAuthorize]
public class AdminMenuItemsController : Controller
{
    private readonly IApiClient _api;

    public AdminMenuItemsController(IApiClient api) => _api = api;

    // === Index ===
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.AdminListMenuItemsAsync(ct);
        return View(items);
    }

    // === Create ===
    [HttpGet]
    public IActionResult Create() => View(new MenuItemEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItemEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.AdminCreateMenuItemAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // === Edit ===
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await _api.AdminGetMenuItemAsync(id, ct);
        if (item is null) return NotFound();

        return View(new MenuItemEditVm
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Category = item.Category,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenuItemEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.AdminUpdateMenuItemAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // === Delete ===
    [HttpGet]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        var item = await _api.AdminGetMenuItemAsync(id, ct);
        if (item is null) return NotFound();

        return View("Delete", item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _api.AdminDeleteMenuItemAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
