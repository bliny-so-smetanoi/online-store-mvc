using Application.Services.IVisitorService.Dtos;

namespace Application.Services.IVisitorService;

public interface IVisitorLoginService
{
    Task<VisitorLoginResult?> LoginAsync(string phone, string code, CancellationToken cancellationToken);
}