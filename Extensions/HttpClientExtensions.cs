using System.Net.Http.Json;
using EasyDine.Web.DTOs;

namespace EasyDine.Web.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T?> GetApiDataAsync<T>(this HttpClient http, string url)
        where T : class
    {
        if (http.BaseAddress is null)
            throw new InvalidOperationException("HttpClient BaseAddress is null, did you use the correct named client?");
        
        var response = await http.GetFromJsonAsync<ApiResponse<T>>(url);
        return response?.Data;
    }
}