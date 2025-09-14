using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.DTOs.MenuItems;

public class MenuItemUpdateDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [Range(0, 99999.99)]
    public decimal? Price { get; set; } 
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string? Category { get; set; }
    
    [MaxLength(2048)]
    public string? ImageUrl { get; set; }
}