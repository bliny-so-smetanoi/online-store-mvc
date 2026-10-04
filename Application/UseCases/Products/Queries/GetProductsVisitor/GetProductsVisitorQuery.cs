using Application.Abstractions;
using Application.Dtos;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;
using Domain.Entities.Products;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Queries.GetProductsVisitor;

public record GetProductsVisitorQuery(GetAllProductsFilterDto Filters, string? SearchString = null) : IQuery<PagedResult<ProductWithImageDto>>;


public class GetProductsVisitorQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductsVisitorQuery, PagedResult<ProductWithImageDto>>
{
    public async Task<PagedResult<ProductWithImageDto>> Handle(GetProductsVisitorQuery request, CancellationToken cancellationToken)
    {
        var searchSpec = ProductSpecification.All();

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            searchSpec &= ProductSpecification.ByName(request.SearchString);
        }
        
        var products = await productRepository.GetAllPagedWithImageAsync(request.Filters, searchSpec, cancellationToken);
        
        return products;
    }
}

