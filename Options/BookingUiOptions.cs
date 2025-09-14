namespace EasyDine.Web.Options;

public sealed class BookingUiOptions
{
    public TimeSpan Opening { get; set; } = new(10, 0, 0);  // 10:00
    public TimeSpan Closing { get; set; } = new(23, 0, 0);  // 23:00
    public int SlotMinutes { get; set; } = 15;

    // (optional, for form hints/validation UI)
    public int MinDurationMinutes { get; set; } = 30;
    public int MaxDurationMinutes { get; set; } = 240;
}