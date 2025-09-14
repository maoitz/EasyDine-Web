using EasyDine.Web.DelegatingHandlers;
using EasyDine.Web.Models;
using EasyDine.Web.Options;
using EasyDine.Web.Services;
using EasyDine.Web.ViewComponents;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<LoggingHandler>();
builder.Services.Configure<HeaderOptions>(builder.Configuration.GetSection("Header"));
builder.Services.Configure<HeroViewComponent.HeroOptions>(builder.Configuration.GetSection("Hero"));
builder.Services.AddMemoryCache();
builder.Services.Configure<BookingUiOptions>(
    builder.Configuration.GetSection("BookingUiOptions"));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Admin/Login";
        o.AccessDeniedPath = "/Admin/Login";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITokenStore, TokenStore>();
builder.Services.AddTransient<AuthHeaderHandler>();

// Register named HttpClient
builder.Services.AddHttpClient<IApiClient, ApiClient>("EasyDineAPI", client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Missing Api:BaseUrl in configuration.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
})
.AddHttpMessageHandler<AuthHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
