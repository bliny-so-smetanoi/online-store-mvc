using Application.Repositories.Orders;
using Application.Services.IVisitorService;
using Application.Services.IVisitorService.Dtos;
using Domain.Entities.Orders;

namespace Infrastructure.Services.VisitorService;

public sealed class LocalVisitorAuthService(IVisitorRepository visitorRepository) : ILocalVisitorAuthService
{
    public async Task<LocalVisitorUser?> ValidateAsync(string phone, string code, CancellationToken cancellationToken)
    {
        var normalizedPhone = Visitor.NormalizePhone(phone);
        if (string.IsNullOrEmpty(normalizedPhone) || string.IsNullOrWhiteSpace(code))
            return null;

        var visitor = await visitorRepository.GetByPhoneAsync(normalizedPhone, cancellationToken);
        var authenticationCode = visitor?.AuthenticationCodes
            .Where(x => x.IsActive(DateTime.Now))
            .MaxBy(x => x.Created);

        if (visitor is null || authenticationCode is null)
            return null;

        var isValid = authenticationCode.TryUse(code.Trim());
        await visitorRepository.UpdateAsync(visitor, cancellationToken);

        if (!isValid)
            return null;

        return new LocalVisitorUser
        {
            Id = visitor.Id.ToString(),
            UserName = $"visitor_{normalizedPhone}",
            Email = string.IsNullOrEmpty(visitor.Email) ? null : visitor.Email,
            FirstName = "Visitor",
            LastName = normalizedPhone
        };
    }
}
