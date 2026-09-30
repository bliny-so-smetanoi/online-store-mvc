using System.Security.Claims;
using KeycloakExtension.Options;
using KeycloakExtension.Services;
using KeycloakExtension.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakExtension;

public static class DependencyInjection
{
    public static void AddKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        var auth = configuration.GetSection("Authentication");

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Forbidden";
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = auth["Authority"];
                options.RequireHttpsMetadata = false; // только для dev
                options.ClientId = auth["ClientId"];
                options.ClientSecret = auth["ClientSecret"];

                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };

                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
            });

        services.Configure<KeycloakProvisioningOptions>(
            configuration.GetSection("KeycloakProvisioning"));

        services.AddHttpClient<IKeycloakAdminClient, KeycloakAdminClient>();
        services.AddHttpClient<IKeycloakTokenClient, KeycloakTokenClient>();
    }
}