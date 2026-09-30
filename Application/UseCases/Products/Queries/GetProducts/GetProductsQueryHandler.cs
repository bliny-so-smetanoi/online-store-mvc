using Application.Abstractions;
using Application.Dtos;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;

namespace Application.UseCases.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository repository) : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllPagedAsync(request.Filter, cancellationToken);
    }
}