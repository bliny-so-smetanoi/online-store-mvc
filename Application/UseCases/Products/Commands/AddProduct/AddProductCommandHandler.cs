using Application.Abstractions;
using Application.Repositories.ICategoryRepository;
using Application.Repositories.IProductRepository;
using Domain.Entities.Products;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Commands.AddProduct;

public class AddProductCommandHandler(IProductRepository repository, ICategoryRepository categories) : ICommandHandler<AddProductCommand, Result>
{
    public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var category = request.Data.CategoryId.HasValue
            ? await categories.GetByIdAsync(request.Data.CategoryId.Value, cancellationToken)
            : null;
        if (request.Data.CategoryId.HasValue && category is null)
            return Result.Failure("CategoryNotFound");

        var images = request.Data.Images.Select(image => Image.Create(image.Content, image.Size, image.FileName)).ToList();
        var product = Product.Create(request.Data.Name, request.Data.Price, request.Data.Description, images, request.Data.Quantity, category);

        
        await repository.AddAsync(product, cancellationToken);
        
        return Result.Success();
    }
}   