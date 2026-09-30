using System.Text.Json;
using KeycloakExtension.Dtos;
using KeycloakExtension.Options;
using KeycloakExtension.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace KeycloakExtension.Services;

public sealed class KeycloakTokenClient : IKeycloakTokenClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly KeycloakProvisioningOptions _options;

    public KeycloakTokenClient(HttpClient httpClient, IOptions<KeycloakProvisioningOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<KeycloakTokenResponse> GetUserTokenAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";

        using var response = await _httpClient.PostAsync(
            url,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _options.UserTokenClientId,
                ["client_secret"] = _options.UserTokenClientSecret,
                ["username"] = username,
                ["password"] = password,
                ["scope"] = "openid profile email"
            }),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<KeycloakTokenResponse>(json, JsonOptions)
               ?? throw new InvalidOperationException("User token response is empty.");
    }
}