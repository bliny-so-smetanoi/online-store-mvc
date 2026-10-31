using Application.Abstractions;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Queries.GetProductDetails;

public record GetProductDetailsQuery(Guid Id) : IQuery<Result<ProductDetailsDto>>;

public class GetProductDetailsQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductDetailsQuery, Result<ProductDetailsDto>>
{
    public async Task<Result<ProductDetailsDto>> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetDetailsAsync(ProductSpecification.ById(request.Id), cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDetailsDto>("Product not found");
        }

        return product;
    }
}
