using System.Globalization;

namespace OnlineStore.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    public LocalizationMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        Localization.FillCulture(context);
        await _next.Invoke(context);
    }
}

public static class Localization {
    public static void FillCulture(HttpContext context) {
        // Read cookie in ASP.NET Core
        var localeCookie = context.Request.Cookies["culture"];

        if (string.IsNullOrEmpty(localeCookie))
        {
            localeCookie = "en"; // default culture

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false
            };

            context.Response.Cookies.Append("culture", localeCookie, options);
            context.Response.Cookies.Append("ui_locales", localeCookie, options);
        }

        context.Request.Headers["Current-Language"] = localeCookie;
        context.Response.Headers["Current-Language"] = localeCookie;

        var culture = CultureInfo.CreateSpecificCulture(localeCookie);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
}