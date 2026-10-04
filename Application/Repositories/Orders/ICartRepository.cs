using Domain.Entities.Orders;

namespace Application.Repositories.Orders;

public interface ICartRepository
{
    Task CreateAsync(Cart cart, CancellationToken cancellationToken);
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Cart>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Cart cart, CancellationToken cancellationToken);
}
