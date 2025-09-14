namespace EasyDine.Web.ViewModels;

public sealed class HeroVm
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required IReadOnlyList<(string Url, string Alt)> Slides { get; init; }
    public string PrimaryHref { get; init; } = "/Menu";
    public string PrimaryText { get; init; } = "Browse Menu";
    public string SecondaryHref { get; init; } = "/About";
    public string SecondaryText { get; init; } = "About Us";
}