using Application.Repositories.Orders;
using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories.Orders;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task CreateAsync(Order order, CancellationToken cancellationToken)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Orders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Orders.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);
    }
}
