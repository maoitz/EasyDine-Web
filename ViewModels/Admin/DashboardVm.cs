using EasyDine.Web.DTOs.Bookings;
using EasyDine.Web.DTOs.MenuItems;

namespace EasyDine.Web.ViewModels.Admin;

public sealed class DashboardVm
{
    public int TotalBookings { get; set; }
    public int TotalTables { get; set; }
    public int TotalMenuItems { get; set; }
    public int PopularItemsCount { get; set; }

    public IReadOnlyList<MenuItemResponseDto> PopularItems { get; set; } = [];
    public IReadOnlyList<BookingResponseDto> RecentBookings { get; set; } = [];
}