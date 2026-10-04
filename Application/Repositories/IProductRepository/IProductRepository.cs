using System.Linq.Expressions;
using Application.Dtos;
using Application.Repositories.ITodoItemRepository.Dtos;
using Application.UseCases.Products.Dtos;
using Domain.Entities.Products;
using Domain.Entities.TodoItem;
using Domain.Specifications.Product;
using NSpecifications;

namespace Application.Repositories.IProductRepository;

public interface IProductRepository
{
    Task<bool> AnyAsync(ASpec<Product> spec,
        CancellationToken cancellationToken = default);
    Task<Product?> GetAsync(ASpec<Product> spec,
        CancellationToken cancellationToken = default,
        params Expression<Func<Product, object>>[] includes);
    Task<PagedResult<ProductDto>> GetAllPagedAsync(GetAllProductsFilterDto? options,
        ASpec<Product> spec,
        CancellationToken cancellationToken = default);
    Task AddAsync(Product product,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ProductWithImageDto>> GetAllPagedWithImageAsync(GetAllProductsFilterDto? options,
        ASpec<Product> spec,
        CancellationToken cancellationToken = default);

    Task<Image?> GetImageAsync(Guid imageId,
        CancellationToken cancellationToken = default);
}