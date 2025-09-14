namespace EasyDine.Web.Models;

public sealed class HeaderOptions
{
    public string CtaText { get; set; } = "Book a table";
    public string CtaHref { get; set; } = "/booking";
    public List<string> Languages { get; set; } = new();
}