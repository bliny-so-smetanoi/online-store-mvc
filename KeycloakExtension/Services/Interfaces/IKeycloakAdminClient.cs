using KeycloakExtension.Dtos;

namespace KeycloakExtension.Services.Interfaces;

public interface IKeycloakAdminClient
{
    Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken);
    Task<KeycloakUserRepresentation?> FindUserByUsernameAsync(string adminToken, string username, CancellationToken cancellationToken);
    Task<string> CreateUserAsync(string adminToken, KeycloakUserRepresentation user, CancellationToken cancellationToken);
    Task ResetPasswordAsync(string adminToken, string userId, string password, CancellationToken cancellationToken);
    Task<KeycloakRoleRepresentation> GetRealmRoleAsync(string adminToken, string roleName, CancellationToken cancellationToken);
    Task AssignRealmRoleAsync(string adminToken, string userId, KeycloakRoleRepresentation role, CancellationToken cancellationToken);
}