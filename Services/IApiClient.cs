using EasyDine.Web.DTOs.Admin;
using EasyDine.Web.DTOs.Bookings;
using EasyDine.Web.DTOs.Customers;
using EasyDine.Web.DTOs.MenuItems;
using EasyDine.Web.DTOs.Tables;
using EasyDine.Web.ViewModels.Admin;

namespace EasyDine.Web.Services;

public interface IApiClient
{
    Task<IReadOnlyList<MenuItemResponseDto>> GetMenuItemsAsync(
        bool? isPopular = null,
        string? category = null,
        string? menu = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,

        CancellationToken ct = default);

    // Admin
    Task<LoginResult> AdminLoginAsync(string username, string password, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItemResponseDto>> AdminGetPopularItemsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BookingResponseDto>> AdminGetRecentBookingsAsync(int take = 5, CancellationToken ct = default);

    // Menu Items
    Task<IReadOnlyList<MenuItemResponseDto>> AdminListMenuItemsAsync(CancellationToken ct = default);
    Task<MenuItemResponseDto?> AdminGetMenuItemAsync(int id, CancellationToken ct = default);
    Task<MenuItemResponseDto?> AdminCreateMenuItemAsync(MenuItemEditVm vm, CancellationToken ct = default);
    Task<bool> AdminUpdateMenuItemAsync(MenuItemEditVm vm, CancellationToken ct = default);
    Task<bool> AdminDeleteMenuItemAsync(int id, CancellationToken ct = default);
    
    // Tables
    Task<IReadOnlyList<TableResponseDto>> AdminListTablesAsync(CancellationToken ct = default);
    Task<TableResponseDto?> AdminGetTableAsync(int id, CancellationToken ct = default);
    Task<TableResponseDto?> AdminCreateTableAsync(TableEditVm vm, CancellationToken ct = default);
    Task<bool> AdminUpdateTableAsync(TableEditVm vm, CancellationToken ct = default);
    Task<bool> AdminDeleteTableAsync(int id, CancellationToken ct = default);
    
    // Bookings
    Task<IReadOnlyList<BookingResponseDto>> AdminListBookingsAsync(DateTime? date = null, CancellationToken ct = default);
    //Task<BookingResponseDto?> AdminGetBookingAsync(int id, CancellationToken ct = default);
    //Task AdminCreateBookingAsync(BookingEditVm vm, CancellationToken ct = default);
    //Task AdminUpdateBookingAsync(BookingEditVm vm, CancellationToken ct = default);
    //Task AdminDeleteBookingAsync(int id, CancellationToken ct = default);
    Task<BookingResponseDto?> AdminGetBookingAsync(int id, CancellationToken ct = default);
    Task<bool> AdminUpdateBookingAsync(int id, BookingUpdateDto dto, CancellationToken ct = default);

    Task<bool> AdminDeleteBookingAsync(int id, CancellationToken ct = default);
    
    // -- Composite operations --
    
    // Customers (for fallback)
    Task<CustomerResponseDto?> AdminCreateCustomerAsync(CustomerCreateDto dto, CancellationToken ct = default);

    // Bookings (raw)
    Task<BookingResponseDto?> AdminCreateBookingAsync(BookingCreateDto dto, CancellationToken ct = default);

    // Composite (tries with-customer, falls back to two-step)
    Task<BookingResponseDto?> AdminCreateBookingWithCustomerAsync(BookingCreateVm vm, CancellationToken ct = default);


}