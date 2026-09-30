using Application.Abstractions;
using Application.Dtos;
using Application.UseCases.Products.Dtos;

namespace Application.UseCases.Products.Queries.GetProducts;

public record GetProductsQuery(GetAllProductsFilterDto Filter) : IQuery<PagedResult<ProductDto>>;