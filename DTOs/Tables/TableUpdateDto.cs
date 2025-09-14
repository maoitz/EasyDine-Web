using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.DTOs.Tables;

public class TableUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [Range(1, 500)]
    public int Number { get; set; }

    [Required]
    [Range(1, 20)]
    public int Seats { get; set; }
}