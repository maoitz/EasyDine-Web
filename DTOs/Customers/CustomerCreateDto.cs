namespace EasyDine.Web.DTOs.Customers;

public sealed class CustomerCreateDto
{
    public required string FirstName { get; set; }
    public required string LastName  { get; set; }
    public required string Email     { get; set; }
    public string? PhoneNumber       { get; set; }
}