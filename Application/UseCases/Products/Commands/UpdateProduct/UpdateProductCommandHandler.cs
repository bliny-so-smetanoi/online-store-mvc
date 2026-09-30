using Application.Abstractions;
using Application.Repositories.ICategoryRepository;
using Application.Repositories.IProductRepository;
using CSharpFunctionalExtensions;
using Domain.Entities.Products;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository repository, ICategoryRepository categories) : ICommandHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetAsync(ProductSpecification.ById(request.Id), cancellationToken, x => x.Images);
        
        if (product is null)
            return Result.Failure("Product not found");

        var category = request.Data.CategoryId.HasValue
            ? await categories.GetByIdAsync(request.Data.CategoryId.Value, cancellationToken)
            : null;
        if (request.Data.CategoryId.HasValue && category is null)
            return Result.Failure("CategoryNotFound");

        var images = request.Data.Images.Select(x => Image.Create(x.Content, x.Size, x.FileName)).ToList();
        product.Update(request.Data.Name, request.Data.Price, request.Data.Description, request.Data.Quantity, images, category);
        
        await repository.UpdateAsync(product, cancellationToken);
        
        return Result.Success();
    }
}