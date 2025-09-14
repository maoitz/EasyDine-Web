namespace EasyDine.Web.DTOs.Bookings;

public sealed class BookingResponseDto
{
    public int Id { get; set; }

    // Booking
    public int TableId { get; set; }
    public int TableNumber { get; set; }
    public DateTime DateBooked { get; set; }
    public int DurationMinutes { get; set; }
    public int TotalGuests { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Customer
    public string CustomerFirstName { get; set; } = string.Empty;
    public string? CustomerLastName { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
}