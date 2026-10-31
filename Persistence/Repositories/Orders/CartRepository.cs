using Application.Repositories.Orders;
using Application.UseCases.Orders.Dtos;
using Domain.Entities.Orders;
using Domain.Enums;
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

    public async Task<List<CartItemDto>> GetActiveCartItemsAsync(Guid visitorId, CancellationToken cancellationToken)
    {
        return await context.Carts
            .Where(x => x.Visitor.Id == visitorId && x.Status == CartStatus.Created && !x.IsDeleted)
            .SelectMany(x => x.Products)
            .OrderBy(x => x.Created)
            .Select(x => new CartItemDto
            {
                ProductId = x.Product.Id,
                Name = x.Product.Name,
                CategoryName = x.Product.Category != null ? x.Product.Category.Name : null,
                Price = x.Product.Price,
                Quantity = x.Quantity,
                ImageId = x.Product.Images.OrderBy(i => i.Created).ThenBy(i => i.Id).Select(i => (Guid?)i.Id).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
    }
}
