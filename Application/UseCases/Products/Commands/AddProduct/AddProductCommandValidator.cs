using Application.Mediator;
using CSharpFunctionalExtensions;
using FluentValidation.Results;

namespace Application.UseCases.Products.Commands.AddProduct;

public class AddProductCommandValidator : RequestValidatorBase<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Data.Name != null);
    }
    
    public override Task<Result> RequestValidateAsync(AddProductCommand request, CancellationToken cancellationToken)
    {
        return base.RequestValidateAsync(request, cancellationToken);
    }
}