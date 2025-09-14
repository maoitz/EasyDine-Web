using EasyDine.Web.Services;
using EasyDine.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EasyDine.Web.ViewComponents;

public sealed class HeroViewComponent : ViewComponent
{
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private readonly IOptions<HeroOptions> _options;
    
    public HeroViewComponent(IApiClient apiClient, IMemoryCache cache, IOptions<HeroOptions> options)
    {
        _apiClient = apiClient;
        _cache = cache;
        _options = options;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var configuration = _options.Value;
        var slidesToTake = Math.Max(1, configuration.Slides);
        
        var nowUtc = DateTimeOffset.UtcNow;
        var tz = GetTimezone(configuration.Timezone);
        var local = TimeZoneInfo.ConvertTime(nowUtc, tz);
        var category = ResolveCategory(local.Hour, configuration);
        
        var cacheKey = $"hero:{category ?? "all"}:n={slidesToTake}";
        var vm = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, configuration.CacheMinutes));

            // Fetch popular items (optionally filtered by category)
            var items = await _apiClient.GetMenuItemsAsync
                (isPopular: true, category: category, ct: HttpContext.RequestAborted);

            // Filter those with images
            var withImages = items
                .Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl))
                .Take(slidesToTake)
                .Select(i => (Url: i.ImageUrl!, Alt: i.Name))
                .ToList();

            // Fill with placeholders if needed
            var result = new List<(string Url, string Alt)>(withImages);
            var placeholders = configuration.Placeholders ?? new();
            var needed = slidesToTake - result.Count;

            for (int i = 0; i < needed && i < placeholders.Count; i++)
            {
                var ph = placeholders[i];
                result.Add((ph, $"Placeholder {i + 1}"));
            }

            // Build the view model
            return new HeroVm
            {
                Title = category is null ? "Build Fast. Stay Consistent." : $"{category} favourites",
                Subtitle = category is null
                    ? "A clean ASP.NET MVC Starter with your data."
                    : $"Top picks for {category.ToLowerInvariant()}",
                Slides = result,
            };
        });
        return View(vm);
    }

    private static TimeZoneInfo GetTimezone(string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            try { 
                return TimeZoneInfo.FindSystemTimeZoneById(id); 
            } 
            catch (TimeZoneNotFoundException) 
            { 
                // Ignore and fallback 
            } 
            catch (InvalidTimeZoneException) 
            { 
                // Ignore and fallback 
            }
        }
        return TimeZoneInfo.Local;
    }
    
    private static string? ResolveCategory(int hour, HeroOptions configuration)
    {
        // Windows are ordered by FromHour ascending; pick the last window where FromHour <= hour
        var windows = (configuration.Windows ?? new()).OrderBy(w => w.FromHour).ToList();
        string? cat = configuration.DefaultCategory;
        foreach (var w in windows)
        {
            if (hour >= w.FromHour) cat = w.Category;
        }
        return cat;
    }

    public sealed class HeroOptions
    {
        public int Slides { get; set; } = 3;
        public int CacheMinutes { get; set; } = 5;
        public string? Timezone { get; set; }
        public List<HeroWindow>? Windows { get; set; }
        public string? DefaultCategory { get; set; }
        public List<string>? Placeholders { get; set; }
    }

    public sealed class HeroWindow
    {
        public int FromHour { get; set; }
        public string Category { get; set; } = "";
    }
}