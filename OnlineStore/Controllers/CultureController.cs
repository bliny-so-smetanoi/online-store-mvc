using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Resource;

namespace OnlineStore.Controllers;

public class CultureController : Controller
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public CultureController(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }
    
    public IActionResult ChangeLanguage(string locale)
    {
        var culture = CultureInfo.CreateSpecificCulture(locale);

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // Correct way to set cookies in ASP.NET Core
        Response.Cookies.Append(
            "culture",
            locale,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

        Response.Cookies.Append(
            "ui_locales",
            locale,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

        return Redirect(Request.Headers["Referer"].ToString());

    }
    
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public IActionResult GetResources([FromQuery] string? culture = null)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            // если надо – меняем культуру вручную
            var ci = new CultureInfo(culture);
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
        }
        
        var result = new Dictionary<string, string>();
        
        foreach (var item in _localizer.GetAllStrings(includeParentCultures: true))
        {
            result[item.Name] = item.Value;
        }

        return Ok(result);
    }
}