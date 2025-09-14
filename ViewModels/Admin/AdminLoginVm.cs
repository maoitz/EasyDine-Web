using System.ComponentModel.DataAnnotations;

namespace EasyDine.Web.ViewModels.Admin;

public sealed class AdminLoginVm
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? Error { get; set; }
}