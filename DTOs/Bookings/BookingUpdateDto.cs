namespace EasyDine.Web.DTOs.Bookings;

public sealed class BookingUpdateDto
{
    public DateTime? DateBooked { get; set; }
    public int?     TotalGuests { get; set; }
    public string?  Status      { get; set; }
    public int?     DurationMinutes { get; set; }
    public int?     TableId { get; set; } // include Table change support
}