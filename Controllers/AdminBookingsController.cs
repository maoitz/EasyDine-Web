using EasyDine.Web.DTOs.Bookings;
using EasyDine.Web.Filters;
using EasyDine.Web.Options;
using EasyDine.Web.Services;
using EasyDine.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace EasyDine.Web.Controllers;

[AdminAuthorize]
public class AdminBookingsController : Controller
{
    private readonly IApiClient _api;
    private readonly BookingUiOptions _opts;

    public AdminBookingsController(IApiClient api, IOptions<BookingUiOptions> opts)
    {
        _api = api;
        _opts = opts.Value;
    }

    // === Index ===
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? date, CancellationToken ct)
    {
        var bookings = await _api.AdminListBookingsAsync(date, ct);
        ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
        return View(bookings);
    }

    // === Create ===
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new BookingCreateVm();
        await LoadTablesAsync(ct);

        ViewBag.TimeSlots = GenerateTimeSlots(_opts.Opening, _opts.Closing, _opts.SlotMinutes);
        ViewBag.SelectedTime = vm.DateBooked.ToString("HH\\:mm");
        ViewBag.SelectedDate = vm.DateBooked.ToString("yyyy-MM-dd");
        ViewBag.MinDuration = _opts.MinDurationMinutes;
        ViewBag.MaxDuration = _opts.MaxDurationMinutes;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateVm vm, CancellationToken ct)
    {
        await LoadTablesAsync(ct);
        ViewBag.TimeSlots = GenerateTimeSlots(_opts.Opening, _opts.Closing, _opts.SlotMinutes);
        ViewBag.MinDuration = _opts.MinDurationMinutes;
        ViewBag.MaxDuration = _opts.MaxDurationMinutes;

        var dateStr = Request.Form["Date"].ToString();
        var timeStr = Request.Form["Time"].ToString();
        if (!string.IsNullOrWhiteSpace(dateStr) && !string.IsNullOrWhiteSpace(timeStr)
                                                && DateTime.TryParse(dateStr, out var date) &&
                                                TimeSpan.TryParse(timeStr, out var time))
        {
            vm.DateBooked = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0,
                DateTimeKind.Local);
        }
        else
        {
            ModelState.AddModelError(nameof(vm.DateBooked), "Please pick a valid date and time.");
        }

        if (!ModelState.IsValid) return View(vm);

        try
        {
            await _api.AdminCreateBookingWithCustomerAsync(vm, ct);
            return RedirectToAction("Dashboard", "Admin");
        }
        catch (InvalidOperationException ex)
        {
            var msg = ex.Message ?? "Failed to create booking.";
            if (msg.Contains("capacity", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.TotalGuests), msg);
            else if (msg.Contains("open", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("closing", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("hours", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.DateBooked), msg);
            else if (msg.Contains("overlap", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("buffer", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.DateBooked),
                    "Selected time conflicts (buffers). Pick another slot.");
            else
                ModelState.AddModelError(string.Empty, msg);

            return View(vm);
        }
    }

    private async Task LoadTablesAsync(CancellationToken ct)
    {
        var tables = await _api.AdminListTablesAsync(ct);
        ViewBag.Tables = tables
            .OrderBy(t => t.Number)
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = $"Table {t.Number} ({t.Seats} seats)" })
            .ToList();
    }

    // === Edit ===
    
    // GET: /AdminBookings/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var b = await _api.AdminGetBookingAsync(id, ct);
        if (b is null) return NotFound();

        var vm = new BookingEditVm
        {
            Id = b.Id,
            CustomerFirstName = b.CustomerFirstName,
            CustomerLastName = b.CustomerLastName,
            CustomerEmail = b.CustomerEmail,
            CustomerPhone = b.CustomerPhone,
            TableId = b.TableId,
            DateBooked = b.DateBooked,
            DurationMinutes = b.DurationMinutes,
            TotalGuests = b.TotalGuests,
            Status = b.Status
        };

        // dropdowns
        var tables = await _api.AdminListTablesAsync(ct);
        vm.Tables = tables.OrderBy(t => t.Number)
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = $"Table {t.Number} ({t.Seats} seats)" })
            .ToList();

        // time slots from options (reuse what you added)
        var opening = new TimeSpan(10, 0, 0);
        var closing = new TimeSpan(23, 0, 0);
        var slotMin = 15;
        vm.TimeSlots = GenerateTimeSlots(opening, closing, slotMin);

        ViewBag.SelectedDate = vm.DateStr;
        ViewBag.SelectedTime = vm.TimeStr;

        return View(vm);
    }

    // POST: /AdminBookings/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookingEditVm vm, CancellationToken ct)
    {
        // Rehydrate dropdowns
        var tables = await _api.AdminListTablesAsync(ct);
        vm.Tables = tables.OrderBy(t => t.Number)
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = $"Table {t.Number} ({t.Seats} seats)" })
            .ToList();
        vm.TimeSlots = GenerateTimeSlots(new TimeSpan(10, 0, 0), new TimeSpan(23, 0, 0), 15);

        // Combine Date + Time into DateBooked
        var dateStr = Request.Form["Date"].ToString();
        var timeStr = Request.Form["Time"].ToString();
        if (!string.IsNullOrWhiteSpace(dateStr) && !string.IsNullOrWhiteSpace(timeStr) &&
            DateTime.TryParse(dateStr, out var date) && TimeSpan.TryParse(timeStr, out var time))
        {
            vm.DateBooked = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0,
                DateTimeKind.Local);
        }
        else
        {
            ModelState.AddModelError(nameof(vm.DateBooked), "Please pick a valid date and time.");
        }

        if (!ModelState.IsValid) return View(vm);

        try
        {
            var dto = new BookingUpdateDto
            {
                DateBooked = vm.DateBooked,
                TotalGuests = vm.TotalGuests,
                Status = vm.Status,
                DurationMinutes = vm.DurationMinutes,
                TableId = vm.TableId
            };

            await _api.AdminUpdateBookingAsync(vm.Id, dto, ct);
            return RedirectToAction("Index");
        }
        catch (InvalidOperationException ex)
        {
            var msg = ex.Message ?? "Failed to update booking.";
            if (msg.Contains("capacity", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.TotalGuests), msg);
            else if (msg.Contains("open", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("closing", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("hours", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.DateBooked), msg);
            else if (msg.Contains("overlap", StringComparison.OrdinalIgnoreCase) ||
                     msg.Contains("buffer", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError(nameof(vm.DateBooked),
                    "Selected time conflicts (buffers). Pick another slot.");
            else
                ModelState.AddModelError(string.Empty, msg);

            return View(vm);
        }
    }
    
    // GET: /AdminBookings/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var b = await _api.AdminGetBookingAsync(id, ct);
        if (b is null) return NotFound();

        // simple confirm using the response dto directly
        return View(b);
    }

    // POST: /AdminBookings/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _api.AdminDeleteBookingAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    // Move to shard helpers?
    private static List<SelectListItem> GenerateTimeSlots(TimeSpan opening, TimeSpan closing, int slotMinutes)
    {
        var slots = new List<SelectListItem>();
        var t = opening;
        while (t <= closing)
        {
            var label = t.ToString(@"hh\:mm");
            slots.Add(new SelectListItem { Value = label, Text = label });
            t = t.Add(TimeSpan.FromMinutes(slotMinutes));
        }

        return slots;
    }
}