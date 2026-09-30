using Application.Services.IVisitorService;
using Application.Services.IVisitorService.Dtos;

namespace Infrastructure.Services.VisitorService;

public sealed class FakeLocalVisitorAuthService : ILocalVisitorAuthService
{
    public Task<LocalVisitorUser?> ValidateAsync(string phone, string code, CancellationToken cancellationToken)
    {
        // Заглушка. Замени на свою логику.
        if (string.IsNullOrWhiteSpace(phone) || code != "123456")
            return Task.FromResult<LocalVisitorUser?>(null);

        var normalizedPhone = phone.Replace("+", "").Replace(" ", "");

        return Task.FromResult<LocalVisitorUser?>(new LocalVisitorUser
        {
            Id = normalizedPhone,
            UserName = $"visitor_{normalizedPhone}",
            Email = $"{normalizedPhone}@visitor.local",
            FirstName = "Visitor",
            LastName = normalizedPhone
        });
    }
}