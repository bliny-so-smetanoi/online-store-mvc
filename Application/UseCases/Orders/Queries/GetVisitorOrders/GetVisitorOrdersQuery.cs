using Application.Abstractions;
using Application.Repositories.Orders;
using Application.UseCases.Orders.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Orders.Queries.GetVisitorOrders;

public record GetVisitorOrdersQuery(Guid VisitorId) : IQuery<Result<List<VisitorOrderDto>>>;

public class GetVisitorOrdersQueryHandler(
    IVisitorRepository visitorRepository,
    IOrderRepository orderRepository) : IQueryHandler<GetVisitorOrdersQuery, Result<List<VisitorOrderDto>>>
{
    public async Task<Result<List<VisitorOrderDto>>> Handle(GetVisitorOrdersQuery request, CancellationToken cancellationToken)
    {
        var visitor = await visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);

        if (visitor is null)
        {
            return Result.Failure<List<VisitorOrderDto>>("Visitor not found");
        }

        return await orderRepository.GetVisitorOrdersAsync(request.VisitorId, cancellationToken);
    }
}
