using Domain.Entities.Orders;

namespace Application.Repositories.Orders;

public interface IVisitorRepository
{
    Task CreateAsync(Visitor visitor, CancellationToken cancellationToken);
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Visitor>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Visitor visitor, CancellationToken cancellationToken);
}
