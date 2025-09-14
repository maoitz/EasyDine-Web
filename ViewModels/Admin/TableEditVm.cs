using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.ViewModels.Admin;

public class TableEditVm
{
    public int Id { get; set; }

    [Required]
    [Range(1, 500)]
    public int Number { get; set; }

    [Required]
    [Range(1, 20)]
    public int Seats { get; set; }
}