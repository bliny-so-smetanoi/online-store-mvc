using Application.Mediator;
using Application.Repositories.IProductRepository;
using CSharpFunctionalExtensions;
using Domain.Specifications.Product;

namespace Application.UseCases.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator(IProductRepository repository) : RequestValidatorBase<UpdateProductCommand>
{
    public override async Task<Result> RequestValidateAsync(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var anyProduct = await repository.AnyAsync(ProductSpecification.ById(request.Id), cancellationToken);
        if (!anyProduct)
        {
            return Result.Failure("Product not found");
        }
        return Result.Success();
    }
}