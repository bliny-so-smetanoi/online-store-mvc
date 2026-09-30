using System.Security.Cryptography;
using Application.Services.IVisitorService;
using Application.Services.IVisitorService.Dtos;
using KeycloakExtension.Dtos;
using KeycloakExtension.Options;
using KeycloakExtension.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.VisitorService;

public sealed class VisitorLoginService : IVisitorLoginService
{
    private readonly ILocalVisitorAuthService _localVisitorAuthService;
    private readonly IKeycloakAdminClient _keycloakAdminClient;
    private readonly IKeycloakTokenClient _keycloakTokenClient;
    private readonly KeycloakProvisioningOptions _options;

    public VisitorLoginService(
        ILocalVisitorAuthService localVisitorAuthService,
        IKeycloakAdminClient keycloakAdminClient,
        IKeycloakTokenClient keycloakTokenClient,
        IOptions<KeycloakProvisioningOptions> options)
    {
        _localVisitorAuthService = localVisitorAuthService;
        _keycloakAdminClient = keycloakAdminClient;
        _keycloakTokenClient = keycloakTokenClient;
        _options = options.Value;
    }

    public async Task<VisitorLoginResult?> LoginAsync(
        string phone,
        string code,
        CancellationToken cancellationToken)
    {
        var localUser = await _localVisitorAuthService.ValidateAsync(phone, code, cancellationToken);
        if (localUser is null)
            return null;

        var adminToken = await _keycloakAdminClient.GetAdminAccessTokenAsync(cancellationToken);

        var existingUser = await _keycloakAdminClient.FindUserByUsernameAsync(
            adminToken,
            localUser.UserName,
            cancellationToken);

        string keycloakUserId;
        var password = GenerateStrongPassword();

        if (existingUser is null)
        {
            keycloakUserId = await _keycloakAdminClient.CreateUserAsync(
                adminToken,
                new KeycloakUserRepresentation
                {
                    Username = localUser.UserName,
                    Enabled = true,
                    EmailVerified = true,
                    Email = localUser.Email,
                    FirstName = localUser.FirstName,
                    LastName = localUser.LastName,
                    Attributes = new Dictionary<string, List<string>>
                    {
                        ["external_user_id"] = new() { localUser.Id },
                        ["auth_source"] = new() { "shop" },
                        ["visitor"] = new() { "true" }
                    }
                },
                cancellationToken);

            await _keycloakAdminClient.ResetPasswordAsync(
                adminToken,
                keycloakUserId,
                password,
                cancellationToken);

            var role = await _keycloakAdminClient.GetRealmRoleAsync(
                adminToken,
                _options.VisitorRealmRole,
                cancellationToken);

            await _keycloakAdminClient.AssignRealmRoleAsync(
                adminToken,
                keycloakUserId,
                role,
                cancellationToken);
        }
        else
        {
            keycloakUserId = existingUser.Id
                             ?? throw new InvalidOperationException("Existing Keycloak user has no id.");

            await _keycloakAdminClient.ResetPasswordAsync(
                adminToken,
                keycloakUserId,
                password,
                cancellationToken);
        }

        var token = await _keycloakTokenClient.GetUserTokenAsync(
            localUser.UserName,
            password,
            cancellationToken);

        return new VisitorLoginResult
        {
            LocalUserId = localUser.Id,
            KeycloakUserId = keycloakUserId,
            UserName = localUser.UserName,
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            Roles = new List<string> { _options.VisitorRealmRole }
        };
    }

    private static string GenerateStrongPassword()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}