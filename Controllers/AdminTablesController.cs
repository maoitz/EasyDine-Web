using EasyDine.Web.DTOs.Tables;
using EasyDine.Web.Filters;
using EasyDine.Web.Services;
using EasyDine.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace EasyDine.Web.Controllers;

[AdminAuthorize]
public class AdminTablesController : Controller
{
    private readonly IApiClient _api;

    public AdminTablesController(IApiClient api) => _api = api;

    // === Index ===
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var tables = await _api.AdminListTablesAsync(ct);
        return View(tables);
    }

    // === Create ===
    [HttpGet]
    public IActionResult Create() => View(new TableEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TableEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.AdminCreateTableAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // === Edit ===
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var table = await _api.AdminGetTableAsync(id, ct);
        if (table is null) return NotFound();

        return View(new TableEditVm
        {
            Id = table.Id,
            Number = table.Number,
            Seats = table.Seats
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TableEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.AdminUpdateTableAsync(vm, ct);
        return RedirectToAction(nameof(Index));
    }

    // === Delete ===
    [HttpGet]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        var table = await _api.AdminGetTableAsync(id, ct);
        if (table is null) return NotFound();

        return View("Delete", table);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _api.AdminDeleteTableAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
