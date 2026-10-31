using System.Security.Claims;
using Application.Services.IVisitorService;
using Application.Services.IVisitorService.Dtos;
using Application.UseCases.Orders.Commands.RequestVisitorCode;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Resource;

namespace OnlineStore.Controllers;

public class AccountController : Controller
{
    private readonly IVisitorLoginService _visitorLoginService;
    private readonly IMediator _mediator;
    private readonly IAntiforgery _antiforgery;

    public AccountController(IVisitorLoginService visitorLoginService, IMediator mediator, IAntiforgery antiforgery)
    {
        _visitorLoginService = visitorLoginService;
        _mediator = mediator;
        _antiforgery = antiforgery;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestVisitorCode([FromBody] VisitorCodeRequestVm model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(CatalogResources.Get("VisitorPhoneInvalid"));

        var result = await _mediator.Send(new RequestVisitorCodeCommand(model.Phone), cancellationToken);
        if (result.IsFailure)
            return BadRequest(CatalogResources.Get(result.Error));

        return Ok();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VisitorLogin([FromBody] VisitorLoginVm model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(CatalogResources.Get("VisitorCodeInvalid"));

        var result = await _visitorLoginService.LoginAsync(
            model.Phone,
            model.Code,
            cancellationToken);

        if (result is null)
            return BadRequest(CatalogResources.Get("VisitorCodeInvalid"));

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
            IsPersistent = true
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

        // Antiforgery-токен привязан к пользователю: выдаём новый, иначе следующие POST после входа получат 400
        HttpContext.User = principal;
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new { requestToken = tokens.RequestToken });
    }

}
