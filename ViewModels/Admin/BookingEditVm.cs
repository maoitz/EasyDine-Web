using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyDine.Web.ViewModels.Admin;

public sealed class BookingEditVm
{
    [Required] public int Id { get; set; }

    // Customer (read-only in Edit for now; changeable later if you want)
    public string CustomerFirstName { get; set; } = string.Empty;
    public string? CustomerLastName { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }

    // Booking
    [Required] public int TableId { get; set; }
    [Required] public DateTime DateBooked { get; set; }
    [Range(15, 300)] public int DurationMinutes { get; set; } = 120;
    [Range(1, 20)]  public int TotalGuests { get; set; } = 2;
    [Required] public string Status { get; set; } = "Pending";

    // UI helpers
    public List<SelectListItem> Tables { get; set; } = new();
    public List<SelectListItem> TimeSlots { get; set; } = new();

    // Helpers for date+time split (not bound to model state)
    public string DateStr => DateBooked.ToString("yyyy-MM-dd");
    public string TimeStr => DateBooked.ToString("HH\\:mm");
}