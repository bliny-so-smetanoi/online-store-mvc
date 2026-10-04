using Application.Abstractions;
using Application.Mediator;
using Application.Repositories.IProductRepository;
using Application.Repositories.Orders;
using CSharpFunctionalExtensions;
using Domain.Entities.Orders;
using Domain.Specifications.Product;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.UseCases.Orders.Commands.AddToCart;

public record AddToCartCommand(Guid ProductId, Guid VisitorId) : ICommand<Result>;

public class AddToCartCommandValidator(
    IProductRepository productRepository,
    IVisitorRepository visitorRepository) : RequestValidatorBase<AddToCartCommand>
{
    public override async Task<Result> RequestValidateAsync(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.GetAsync(ProductSpecification.ById(request.ProductId), cancellationToken);

        if (productExists is null)
        {
            return Result.Failure("Product not found");
        }
        
        var visitorExists = await visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);

        if (visitorExists is null)
        {
            return Result.Failure("Visitor not found"); 
        }
        
        return Result.Success();
    }
}

public class AddToCartCommandHandler(
    IProductRepository productRepository,
    IVisitorRepository visitorRepository,
    ICartRepository cartRepository) : ICommandHandler<AddToCartCommand, Result>
{
    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(ProductSpecification.ById(request.ProductId), cancellationToken);
        var visitor = await visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);
        
        var cart = visitor!.GetActiveCart();

        if (cart is null)
        {
            var newCart = Cart.Create(visitor);
            newCart.AddProduct(product!);
            await cartRepository.CreateAsync(newCart, cancellationToken);
            
            return Result.Success();
        }
        
        cart.AddProduct(product!);
        
        await cartRepository.UpdateAsync(cart, cancellationToken);
        
        return Result.Success();
    }
}
