using Application.Abstractions;
using Application.Repositories.IProductRepository;
using Domain.Entities.Products;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Commands.AddProduct;

public class AddProductCommandHandler(IProductRepository repository) : ICommandHandler<AddProductCommand, Result>
{
    public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var images = request.Data.Images.Select(image => Image.Create(image.Content, image.Size, image.FileName)).ToList();
        var product = Product.Create(request.Data.Name, request.Data.Price, request.Data.Description, images, request.Data.Quantity, null); 
        //TODO: implement Category
        
        await repository.AddAsync(product, cancellationToken);
        
        return Result.Success();
    }
}   