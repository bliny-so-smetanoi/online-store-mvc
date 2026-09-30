using KeycloakExtension.Dtos;

namespace KeycloakExtension.Services.Interfaces;

public interface IKeycloakTokenClient
{
    Task<KeycloakTokenResponse> GetUserTokenAsync(string username, string password, CancellationToken cancellationToken);
}