using System.Security.Cryptography;
using Application.Abstractions;
using Application.Repositories.Orders;
using Application.Services.IVisitorService;
using CSharpFunctionalExtensions;
using Domain.Entities.Orders;

namespace Application.UseCases.Orders.Commands.RequestVisitorCode;

public record RequestVisitorCodeCommand(string Phone) : ICommand<Result>;

public class RequestVisitorCodeCommandHandler(
    IVisitorRepository visitorRepository,
    IVisitorCodeSender visitorCodeSender) : ICommandHandler<RequestVisitorCodeCommand, Result>
{
    private static readonly TimeSpan ResendInterval = TimeSpan.FromMinutes(1);

    public async Task<Result> Handle(RequestVisitorCodeCommand request, CancellationToken cancellationToken)
    {
        var phone = Visitor.NormalizePhone(request.Phone);
        if (phone.Length is < 10 or > 15)
            return Result.Failure("VisitorPhoneInvalid");

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var visitor = await visitorRepository.GetByPhoneAsync(phone, cancellationToken);

        if (visitor is null)
        {
            // Первый вход: визитор создаётся вместе с первым кодом
            visitor = Visitor.Create(phone, string.Empty);
            visitor.AddAuthenticationCode(code);
            await visitorRepository.CreateAsync(visitor, cancellationToken);
        }
        else
        {
            if (visitor.AuthenticationCodes.Any(x => !x.IsDeleted && x.Created.Add(ResendInterval) > DateTime.Now))
                return Result.Failure("VisitorCodeRequestTooOften");

            visitor.AddAuthenticationCode(code);
            await visitorRepository.UpdateAsync(visitor, cancellationToken);
        }

        await visitorCodeSender.SendAsync(phone, code, cancellationToken);

        return Result.Success();
    }
}
