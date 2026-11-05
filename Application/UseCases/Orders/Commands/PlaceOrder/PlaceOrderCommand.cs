using System.Globalization;
using Application.Abstractions;
using Application.Mediator;
using Application.Repositories.Orders;
using Application.UseCases.Orders.Dtos;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.UseCases.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(Guid VisitorId, PlaceOrderDto Data) : ICommand<Result>;

public class PlaceOrderCommandHandler(
    IVisitorRepository visitorRepository,
    IOrderRepository orderRepository) : ICommandHandler<PlaceOrderCommand, Result>
{
    public async Task<Result> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var visitor = await visitorRepository.GetByIdAsync(request.VisitorId,
            cancellationToken);

        var activeCart = visitor!.GetActiveCart();

        var data = request.Data;
        var address = new Address(data.Country.Trim(), data.City.Trim(), data.Street.Trim(), data.PostalCode.Trim());
        var paymentInfo = PaymentInfo.Create(data.CardNumber, data.ExpirationDate, data.CardHolderName);

        var order = activeCart!.PlaceOrder(address, paymentInfo);

        await orderRepository.CreateAsync(order, cancellationToken);

        return Result.Success();
    }
}


public class PlaceOrderCommandValidator : RequestValidatorBase<PlaceOrderCommand>
{
    private readonly IVisitorRepository _visitorRepository;

    public PlaceOrderCommandValidator(IVisitorRepository visitorRepository)
    {
        _visitorRepository = visitorRepository;

        RuleFor(x => x.Data).NotNull().WithMessage("Order data is required");

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.Country).NotEmpty().WithMessage("Enter a country")
                .MaximumLength(100).WithMessage("Country is too long");
            RuleFor(x => x.Data.City).NotEmpty().WithMessage("Enter a city")
                .MaximumLength(100).WithMessage("City is too long");
            RuleFor(x => x.Data.Street).NotEmpty().WithMessage("Enter a street address")
                .MaximumLength(250).WithMessage("Street address is too long");
            RuleFor(x => x.Data.PostalCode).NotEmpty().WithMessage("Enter a postal code")
                .MaximumLength(20).WithMessage("Postal code is too long");

            RuleFor(x => x.Data.CardHolderName).NotEmpty().WithMessage("Enter the card holder name")
                .MaximumLength(100).WithMessage("Card holder name is too long");
            RuleFor(x => x.Data.CardNumber).Must(BeValidCardNumber).WithMessage("Invalid card number");
            RuleFor(x => x.Data.ExpirationDate).Must(BeValidExpirationDate).WithMessage("Invalid or expired card expiration date");
            RuleFor(x => x.Data.Cvv).Matches(@"^\d{3,4}$").WithMessage("Invalid CVV");
        });
    }

    public override async Task<Result> RequestValidateAsync(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var visitorExists = await _visitorRepository.GetByIdAsync(request.VisitorId, cancellationToken);
        if (visitorExists is null)
        {
            return Result.Failure("Visitor not found");
        }

        var activeCart = visitorExists.GetActiveCart();

        if (activeCart is null || activeCart.Products.Count == 0)
        {
            return Result.Failure("No active cart");
        }

        var outOfStock = activeCart.Products.FirstOrDefault(x => x.Quantity > x.Product.Quantity);
        if (outOfStock is not null)
        {
            return Result.Failure($"Not enough product in stock: {outOfStock.Product.Name}");
        }

        return Result.Success();
    }

    // 12–19 цифр (пробелы и дефисы допускаются) + контрольная сумма Луна
    private static bool BeValidCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Any(c => !char.IsDigit(c) && c != ' ' && c != '-'))
            return false;

        var digits = cardNumber.Where(char.IsDigit).Select(c => c - '0').ToArray();
        if (digits.Length is < 12 or > 19)
            return false;

        var sum = 0;
        for (var i = 0; i < digits.Length; i++)
        {
            var digit = digits[digits.Length - 1 - i];
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }

        return sum % 10 == 0;
    }

    private static bool BeValidExpirationDate(string? expirationDate)
    {
        if (!DateTime.TryParseExact(expirationDate?.Trim(), "MM/yy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
            return false;

        var today = DateTime.Today;
        return date.Year > today.Year || (date.Year == today.Year && date.Month >= today.Month);
    }
}
