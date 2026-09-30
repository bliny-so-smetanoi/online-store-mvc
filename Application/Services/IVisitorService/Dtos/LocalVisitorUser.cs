namespace Application.Services.IVisitorService.Dtos;

public sealed class LocalVisitorUser
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}