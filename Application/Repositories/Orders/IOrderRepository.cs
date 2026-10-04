using Domain.Entities.Orders;

namespace Application.Repositories.Orders;

public interface IOrderRepository
{
    Task CreateAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
}
