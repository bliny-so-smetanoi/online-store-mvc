using Application.Abstractions;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Queries.GetProductById;

public class GetProductForUpdateByIdQueryHandler(IProductRepository repository) : IQueryHandler<GetProductForUpdateByIdQuery, Result<UpdateViewProductDto>>
{
    public async Task<Result<UpdateViewProductDto>> Handle(GetProductForUpdateByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetAsync(ProductSpecification.ById(request.Id), cancellationToken, x => x.Images);
        if (product is null)
        {
            return Result.Failure<UpdateViewProductDto>("Product not found");
        }
        return new UpdateViewProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Images = product.Images.Select(x => new ImageDto
            {
                Content = x.Content,
                FileName = x.FileName,
                Size = x.Size
            }).ToList()
        };
    }
}