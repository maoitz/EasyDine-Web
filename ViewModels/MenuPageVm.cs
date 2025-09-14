using EasyDine.Web.DTOs.MenuItems;

public sealed class MenuSectionVm
{
    public required string Id { get; init; }        // "lunch", "dinner", "drinks"
    public required string Title { get; init; }     // "Lunch", "Dinner", "Drinks"
    public required List<MenuItemResponseDto> Items { get; init; }
}

public sealed class MenuPageVm
{
    public required List<MenuSectionVm> Sections { get; init; }
}