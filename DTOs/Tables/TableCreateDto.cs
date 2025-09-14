using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.DTOs.Tables;

public class TableCreateDto
{
    [Required]
    [Range(1, 500)]
    public int Number { get; set; }

    [Required]
    [Range(1, 20)]
    public int Seats { get; set; }
}