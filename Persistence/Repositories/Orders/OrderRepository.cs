using Application.Repositories.Orders;
using Application.UseCases.Orders.Dtos;
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

    public async Task<List<VisitorOrderDto>> GetVisitorOrdersAsync(Guid visitorId, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(x => x.Cart.Visitor.Id == visitorId && !x.IsDeleted)
            .OrderByDescending(x => x.Created)
            .Select(x => new VisitorOrderDto
            {
                OrderId = x.Id,
                Created = x.Created,
                Status = x.Status,
                Country = x.Address.Country,
                City = x.Address.City,
                Street = x.Address.Street,
                PostalCode = x.Address.PostalCode,
                MaskedCardNumber = x.PaymentInfo.MaskedCardNumber,
                CardHolderName = x.PaymentInfo.CardHolderName,
                ExpirationDate = x.PaymentInfo.ExpirationDate,
                Items = x.Cart.Products
                    .OrderBy(p => p.Created)
                    .Select(p => new CartItemDto
                    {
                        ProductId = p.Product.Id,
                        Name = p.Product.Name,
                        CategoryName = p.Product.Category != null ? p.Product.Category.Name : null,
                        Price = p.Product.Price,
                        Quantity = p.Quantity,
                        ImageId = p.Product.Images.OrderBy(i => i.Created).ThenBy(i => i.Id).Select(i => (Guid?)i.Id).FirstOrDefault()
                    })
                    .ToList()
            })
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }
}
