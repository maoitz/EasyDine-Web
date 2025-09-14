using System.Net;
using System.Web;
using EasyDine.Web.DTOs.Admin;
using EasyDine.Web.DTOs.Bookings;
using EasyDine.Web.DTOs.Customers;
using EasyDine.Web.DTOs.MenuItems;
using EasyDine.Web.DTOs.Tables;
using EasyDine.Web.ViewModels.Admin;


namespace EasyDine.Web.Services;

public sealed class ApiClient(HttpClient httpClient) : IApiClient
{
    public async Task<IReadOnlyList<MenuItemResponseDto>> GetMenuItemsAsync(
        bool? isPopular = null, string? category = null, string? menu = null,
        decimal? minPrice = null, decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        var qb = HttpUtility.ParseQueryString(string.Empty);
        if (category is not null) qb["category"] = category;
        if (menu is not null) qb["menu"] = menu;
        if (isPopular.HasValue) qb["isPopular"] = isPopular.Value.ToString().ToLowerInvariant();
        if (minPrice.HasValue) qb["minPrice"] = minPrice.Value.ToString();
        if (maxPrice.HasValue) qb["maxPrice"] = maxPrice.Value.ToString();

        var qs = qb.Count > 0 ? "?" + qb : string.Empty;
        var envelope = await httpClient
            .GetFromJsonAsync<ApiEnvelope<IEnumerable<MenuItemResponseDto>>>("menuitems" + qs, ct);

        return envelope?.Data?.ToList() ?? new List<MenuItemResponseDto>();
    }

    public async Task<LoginResult> AdminLoginAsync(string username, string password, CancellationToken ct = default)
    {
        var res = await httpClient.PostAsJsonAsync("auth/login", new { username, password }, ct);
        if (!res.IsSuccessStatusCode)
            return new LoginResult { Success = false, Error = "Invalid credentials." };

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<string>>(cancellationToken: ct);
        return env is not null && env.Success && !string.IsNullOrWhiteSpace(env.Data)
            ? new LoginResult { Success = true, AccessToken = env.Data }
            : new LoginResult { Success = false, Error = env?.Message ?? "Login failed." };
    }

    // LIST: Menu Items
    public async Task<IReadOnlyList<MenuItemResponseDto>> AdminListMenuItemsAsync(CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<IEnumerable<MenuItemResponseDto>>>(
            "menuitems", ct);

        return env?.Data?.ToList() ?? new List<MenuItemResponseDto>();
    }

    // LIST: Bookings
    public async Task<IReadOnlyList<BookingResponseDto>> AdminListBookingsAsync(DateTime? date = null, CancellationToken ct = default)
    {
        var path = date.HasValue
            ? $"bookings?date={date.Value:yyyy-MM-dd}"
            : "bookings";
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<IEnumerable<BookingResponseDto>>>(path, ct);
        return env?.Data?.ToList() ?? new List<BookingResponseDto>();
    }

    // LIST: Tables
    public async Task<IReadOnlyList<TableResponseDto>> AdminListTablesAsync(CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<IEnumerable<TableResponseDto>>>(
            "tables", ct);

        return env?.Data?.ToList() ?? new List<TableResponseDto>();
    }

    // GET: Bookings
    public async Task<IReadOnlyList<BookingResponseDto>> AdminGetRecentBookingsAsync(int take = 5,
        CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<IEnumerable<BookingResponseDto>>>(
            $"bookings?take={take}", ct);

        return env?.Data?.ToList() ?? new List<BookingResponseDto>();
    }

    // GET: Popular Items
    public async Task<IReadOnlyList<MenuItemResponseDto>> AdminGetPopularItemsAsync(CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<IEnumerable<MenuItemResponseDto>>>(
            "menuitems?isPopular=true", ct);
        return env?.Data?.ToList() ?? new List<MenuItemResponseDto>();
    }

    // === Admin: Menu Items CRUD ===

    // GET /api/menuitems/{id}
    public async Task<MenuItemResponseDto?> AdminGetMenuItemAsync(int id, CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<MenuItemResponseDto>>($"menuitems/{id}", ct);
        return env?.Data;
    }

    // POST /api/menuitems
    public async Task<MenuItemResponseDto?> AdminCreateMenuItemAsync(MenuItemEditVm vm, CancellationToken ct = default)
    {
        var dto = new MenuItemCreateDto
        {
            Name = vm.Name,
            Price = vm.Price,
            Description = vm.Description,
            Category = vm.Category,
            ImageUrl = vm.ImageUrl
        };

        var res = await httpClient.PostAsJsonAsync("menuitems", dto, ct);

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Create failed: {res.StatusCode} - {error}");
        }

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<MenuItemResponseDto>>(cancellationToken: ct);
        return env?.Data;
    }

    // PATCH /api/menuitems/{id}
    public async Task<bool> AdminUpdateMenuItemAsync(MenuItemEditVm vm, CancellationToken ct = default)
    {
        if (vm.Id is null) return false;

        var dto = new MenuItemUpdateDto
        {
            Name = vm.Name,
            Price = vm.Price,
            Description = vm.Description,
            Category = vm.Category,
            ImageUrl = vm.ImageUrl
        };

        var res = await httpClient.PatchAsJsonAsync($"menuitems/{vm.Id}", dto, ct);

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Update failed: {res.StatusCode} - {error}");
        }

        return true;
    }

    // DELETE /api/menuitems/{id}
    public async Task<bool> AdminDeleteMenuItemAsync(int id, CancellationToken ct = default)
    {
        var res = await httpClient.DeleteAsync($"menuitems/{id}", ct);
        return res.IsSuccessStatusCode;
    }

    // === Admin: Tables CRUD ===

    // GET /api/tables/{id}
    public async Task<TableResponseDto?> AdminGetTableAsync(int id, CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<TableResponseDto>>($"tables/{id}", ct);
        return env?.Data;
    }

    // POST /api/tables
    public async Task<TableResponseDto?> AdminCreateTableAsync(TableEditVm vm, CancellationToken ct = default)
    {
        var dto = new TableCreateDto
        {
            Number = vm.Number,
            Seats = vm.Seats
        };

        var res = await httpClient.PostAsJsonAsync("tables", dto, ct);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Create table failed: {res.StatusCode} - {error}");
        }

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<TableResponseDto>>(cancellationToken: ct);
        return env?.Data;
    }

    // PUT /api/tables/{id}
    public async Task<bool> AdminUpdateTableAsync(TableEditVm vm, CancellationToken ct = default)
    {
        var dto = new TableUpdateDto
        {
            Id = vm.Id,
            Number = vm.Number,
            Seats = vm.Seats
        };

        var res = await httpClient.PutAsJsonAsync($"tables/{vm.Id}", dto, ct);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Update table failed: {res.StatusCode} - {error}");
        }

        return true;
    }

    // DELETE /api/tables/{id}
    public async Task<bool> AdminDeleteTableAsync(int id, CancellationToken ct = default)
    {
        var res = await httpClient.DeleteAsync($"tables/{id}", ct);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Delete table failed: {res.StatusCode} - {error}");
        }

        return true;
    }
    
    // === Admin: Bookings CRUD ===
    
    // GET /api/bookings/{id}
    
    
    // --- Customers (fallback support) ---
    public async Task<CustomerResponseDto?> AdminCreateCustomerAsync(CustomerCreateDto dto, CancellationToken ct = default)
    {
        var res = await httpClient.PostAsJsonAsync("customers", dto, ct);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Create customer failed: {res.StatusCode} - {error}");
        }

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<CustomerResponseDto>>(cancellationToken: ct);
        return env?.Data;
    }
    
    // --- Bookings (raw POST /bookings) ---
    public async Task<BookingResponseDto?> AdminCreateBookingAsync(BookingCreateDto dto, CancellationToken ct = default)
    {
        var res = await httpClient.PostAsJsonAsync("bookings", dto, ct);
        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Create booking failed: {res.StatusCode} - {error}");
        }

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<BookingResponseDto>>(cancellationToken: ct);
        return env?.Data;
    }
    
    // --- Composite: try /bookings/with-customer, else fallback to two-step ---
    public async Task<BookingResponseDto?> AdminCreateBookingWithCustomerAsync(BookingCreateVm vm, CancellationToken ct = default)
    {
        // FLAT payload to match POST /api/bookings/with-customer
        var dto = new
        {
            // customer
            firstName       = vm.FirstName,
            lastName        = vm.LastName,
            email           = vm.Email,
            phone           = vm.PhoneNumber,

            // booking
            tableId         = vm.TableId,
            dateBooked      = vm.DateBooked,
            durationMinutes = vm.DurationMinutes,
            totalGuests     = vm.TotalGuests
        };

        var res = await httpClient.PostAsJsonAsync("bookings/with-customer", dto, ct);
        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<BookingResponseDto>>(cancellationToken: ct);

        if (res.IsSuccessStatusCode) return env?.Data;

        var error = await res.Content.ReadAsStringAsync(ct);
        throw new InvalidOperationException($"Create booking (with-customer) failed: {res.StatusCode} - {error}");
    }
    
    // GET /api/bookings/{id}
    public async Task<BookingResponseDto?> AdminGetBookingAsync(int id, CancellationToken ct = default)
    {
        var env = await httpClient.GetFromJsonAsync<ApiEnvelope<BookingResponseDto>>($"bookings/{id}", ct);
        return env?.Data;
    }

    // PUT /api/bookings/{id}
    public async Task<bool> AdminUpdateBookingAsync(int id, BookingUpdateDto dto, CancellationToken ct = default)
    {
        var res = await httpClient.PutAsJsonAsync($"bookings/{id}", dto, ct);
        if (res.IsSuccessStatusCode) return true;

        var error = await res.Content.ReadAsStringAsync(ct);
        throw new InvalidOperationException($"Update booking failed: {res.StatusCode} - {error}");
    }
    
    public async Task<bool> AdminDeleteBookingAsync(int id, CancellationToken ct = default)
    {
        var res = await httpClient.DeleteAsync($"bookings/{id}", ct);
        if (res.IsSuccessStatusCode) return true;

        var error = await res.Content.ReadAsStringAsync(ct);
        throw new InvalidOperationException($"Delete booking failed: {res.StatusCode} - {error}");
    }


    private sealed class ApiEnvelope<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

    }
}