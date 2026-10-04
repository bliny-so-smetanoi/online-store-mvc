using Application.Abstractions;
using Application.Mediator;
using Application.Repositories.IProductRepository;
using Application.Repositories.Orders;
using CSharpFunctionalExtensions;
using Domain.Specifications.Product;

namespace Application.UseCases.Orders.Commands.RemoveFromCart;

public record RemoveFromCartCommand(Guid ProductId, Guid VisitorId) : ICommand<Result>;

public class RemoveFromCartCommandValidator(
    IProductRepository productRepository,
    IVisitorRepository visitorRepository) : RequestValidatorBase<RemoveFromCartCommand>
{
    public override async Task<Result> RequestValidateAsync(RemoveFromCartCommand request, CancellationToken cancellationToken)
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
        
        var activeCartExists = visitorExists.GetActiveCart();

        if (activeCartExists is null)
        {
            return Result.Failure("Cart not found");
        }
        
        return Result.Success();
    }
}

public class RemoveFromCartCommandHandler(
    IProductRepository productRepository,
    IVisitorRepository visitorRepository,
    ICartRepository cartRepository) : ICommandHandler<RemoveFromCartCommand, Result>
{
    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(ProductSpecification.ById(request.ProductId), cancellationToken);
        var visitor = await visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);
        
        var cart = visitor!.GetActiveCart();
        
        cart!.RemoveProduct(product!);        
        
        await cartRepository.UpdateAsync(cart, cancellationToken);
        
        return Result.Success();
    }
}