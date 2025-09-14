namespace EasyDine.Web.DTOs.MenuItems;

public class MenuItemResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsPopular { get; init; }
}