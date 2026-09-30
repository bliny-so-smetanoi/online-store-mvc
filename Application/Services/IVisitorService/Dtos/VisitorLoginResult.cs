namespace Application.Services.IVisitorService.Dtos;

public sealed class VisitorLoginResult
{
    public string LocalUserId { get; set; } = default!;
    public string KeycloakUserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public List<string> Roles { get; set; } = new();
}