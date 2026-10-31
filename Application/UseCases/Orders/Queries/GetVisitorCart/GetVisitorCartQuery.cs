using Application.Abstractions;
using Application.Repositories.Orders;
using Application.UseCases.Orders.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Orders.Queries.GetVisitorCart;

public record GetVisitorCartQuery(Guid VisitorId) : IQuery<Result<VisitorCartDto>>;

public class GetVisitorCartQueryHandler(
    IVisitorRepository visitorRepository,
    ICartRepository cartRepository) : IQueryHandler<GetVisitorCartQuery, Result<VisitorCartDto>>
{
    public async Task<Result<VisitorCartDto>> Handle(GetVisitorCartQuery request, CancellationToken cancellationToken)
    {
        var visitor = await visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);

        if (visitor is null)
        {
            return Result.Failure<VisitorCartDto>("Visitor not found");
        }

        var items = await cartRepository.GetActiveCartItemsAsync(request.VisitorId, cancellationToken);

        return new VisitorCartDto
        {
            VisitorId = visitor.Id,
            Phone = visitor.Phone,
            Email = string.IsNullOrEmpty(visitor.Email) ? null : visitor.Email,
            RegisteredAt = visitor.Created,
            Items = items
        };
    }
}
