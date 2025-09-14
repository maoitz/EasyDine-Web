namespace EasyDine.Web.DTOs.Bookings;

public sealed class BookingCreateDto
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public DateTime DateBooked { get; set; }
    public int DurationMinutes { get; set; }
    public int TotalGuests { get; set; }
}