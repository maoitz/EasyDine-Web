namespace EasyDine.Web.DTOs.Customers;

public sealed class CustomerResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";
    public string Email     { get; set; } = "";
    public string? PhoneNumber { get; set; }
}