using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EasyDine.Web.Models;

namespace EasyDine.Web.Controllers;

public record HeroSlide(string src, string alt, string? caption);

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var slides = _configuration.GetSection("HeroSlides").Get<List<HeroSlide>>() ?? new();
        return View(slides);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
