using Application.Repositories.Orders;
using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories.Orders;

public class CartRepository(AppDbContext context) : ICartRepository
{
    public async Task CreateAsync(Cart cart, CancellationToken cancellationToken)
    {
        await context.Carts.AddAsync(cart, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Carts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Cart>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Carts.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken)
    {
        context.Carts.Update(cart);
        await context.SaveChangesAsync(cancellationToken);
    }
}
