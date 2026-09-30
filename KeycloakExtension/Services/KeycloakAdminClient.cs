using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KeycloakExtension.Dtos;
using KeycloakExtension.Options;
using KeycloakExtension.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace KeycloakExtension.Services;

public sealed class KeycloakAdminClient : IKeycloakAdminClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly KeycloakProvisioningOptions _options;

    public KeycloakAdminClient(HttpClient httpClient, IOptions<KeycloakProvisioningOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";

        using var response = await _httpClient.PostAsync(
            url,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.AdminClientId,
                ["client_secret"] = _options.AdminClientSecret
            }),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var token = JsonSerializer.Deserialize<KeycloakTokenResponse>(json, JsonOptions)
                    ?? throw new InvalidOperationException("Admin token response is empty.");

        return token.AccessToken;
    }

    public async Task<KeycloakUserRepresentation?> FindUserByUsernameAsync(
        string adminToken,
        string username,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users?username={Uri.EscapeDataString(username)}&exact=true";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonSerializer.Deserialize<List<KeycloakUserRepresentation>>(json, JsonOptions) ?? new();

        return users.FirstOrDefault();
    }

    public async Task<string> CreateUserAsync(
        string adminToken,
        KeycloakUserRepresentation user,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(user, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException($"Keycloak user '{user.Username}' already exists.");

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString()
                       ?? throw new InvalidOperationException("Keycloak did not return Location header for created user.");

        return location.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
    }

    public async Task ResetPasswordAsync(
        string adminToken,
        string userId,
        string password,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}/reset-password";

        var payload = new KeycloakCredentialRepresentation
        {
            Type = "password",
            Value = password,
            Temporary = false
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<KeycloakRoleRepresentation> GetRealmRoleAsync(
        string adminToken,
        string roleName,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/roles/{Uri.EscapeDataString(roleName)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<KeycloakRoleRepresentation>(json, JsonOptions)
               ?? throw new InvalidOperationException($"Realm role '{roleName}' not found.");
    }

    public async Task AssignRealmRoleAsync(
        string adminToken,
        string userId,
        KeycloakRoleRepresentation role,
        CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new[] { role }, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        // Обычно успех = 204 NoContent
        if (response.IsSuccessStatusCode)
            return;

        response.EnsureSuccessStatusCode();
    }
}