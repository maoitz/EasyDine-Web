using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.ViewModels.Admin;

public class MenuItemEditVm
{
    public int? Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [Range(0, 10000)]
    public decimal Price { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    [Range(0, 1000)]
    public int PopularityThreshold { get; set; } = 50;
}