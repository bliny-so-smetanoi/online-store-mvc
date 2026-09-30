using Application.Services.IVisitorService.Dtos;

namespace Application.Services.IVisitorService;

public interface ILocalVisitorAuthService
{
    Task<LocalVisitorUser?> ValidateAsync(string phone, string code, CancellationToken cancellationToken);
}