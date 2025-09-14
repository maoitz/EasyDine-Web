namespace EasyDine.Web.DTOs.Bookings;

public sealed class BookingWithCustomerCreateDto
{
    public required CustomerDto Customer { get; set; }
    public required BookingDto Booking { get; set; }

    public sealed class CustomerDto
    {
        public required string FirstName { get; set; }
        public required string LastName  { get; set; }
        public required string Email     { get; set; }
        public string? PhoneNumber       { get; set; }
    }

    public sealed class BookingDto
    {
        public required int TableId { get; set; }
        public required DateTime DateBooked { get; set; }
        public required int DurationMinutes { get; set; }
        public required int TotalGuests { get; set; }
        public string Status { get; set; } = "Pending";
    }
}