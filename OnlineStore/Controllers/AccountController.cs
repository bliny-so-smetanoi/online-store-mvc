using System.Security.Claims;
using Application.Services.IVisitorService;
using Application.Services.IVisitorService.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers;

public class AccountController : Controller
{
    private readonly IVisitorLoginService _visitorLoginService;

    public AccountController(IVisitorLoginService visitorLoginService)
    {
        _visitorLoginService = visitorLoginService;
    }

    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, 
            "oidc");
    }
    
    public IActionResult Logout()
    {
        var authSource = User.FindFirst("auth_source")?.Value;

        if (authSource == "visitor")
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            "oidc");

    }
    
    public IActionResult Visitor()
    {
        return View("VisitorLogin");
    }
    
    [HttpPost]
    public async Task<IActionResult> VisitorLogin(VisitorLoginVm model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _visitorLoginService.LoginAsync(
            model.Phone,
            model.Code,
            HttpContext.RequestAborted);

        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Неверный код или пользователь не найден.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.LocalUserId),
            new Claim(ClaimTypes.Name, result.UserName),
            new Claim("kc_user_id", result.KeycloakUserId),
            new Claim("auth_source", "visitor")
        };

        foreach (var role in result.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = "/" 
        };

        authProperties.StoreTokens(new[]
        {
            new AuthenticationToken
            {
                Name = "access_token",
                Value = result.AccessToken
            },
            new AuthenticationToken
            {
                Name = "refresh_token",
                Value = result.RefreshToken ?? string.Empty
            }
        });

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        return LocalRedirect(authProperties.RedirectUri ?? "/");
    }

}