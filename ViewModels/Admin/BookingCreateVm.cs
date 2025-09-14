using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.ViewModels.Admin;

public sealed class BookingCreateVm
{
    // Customer (freeform)
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName  { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    // Booking
    [Required] public int TableId { get; set; }
    [Required] public DateTime DateBooked { get; set; } = DateTime.Now.AddHours(1);
    [Range(15, 300)] public int DurationMinutes { get; set; } = 120;
    [Range(1, 20)]  public int TotalGuests { get; set; } = 2;
    public string Status { get; set; } = "Pending";
}