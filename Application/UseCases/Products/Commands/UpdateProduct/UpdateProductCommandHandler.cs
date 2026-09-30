using Application.Abstractions;
using Application.Repositories.IProductRepository;
using CSharpFunctionalExtensions;
using Domain.Entities.Products;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository repository) : ICommandHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetAsync(ProductSpecification.ById(request.Id), cancellationToken, x => x.Images);
        
        var images = request.Data.Images.Select(x => Image.Create(x.Content, x.Size, x.FileName)).ToList();
        product!.Update(request.Data.Name, request.Data.Price, request.Data.Description, request.Data.Quantity, images);
        
        await repository.UpdateAsync(product, cancellationToken);
        
        return Result.Success();
    }
}